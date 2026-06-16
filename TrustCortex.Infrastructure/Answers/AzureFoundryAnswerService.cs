using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Answers;

public sealed class AzureFoundryAnswerService : IAnswerService
{
    private const string MissingConfigurationMessage =
        "AzureFoundry answer provider is selected, but Endpoint/ApiKey/DeploymentName is missing.";
    private const string InsufficientApprovedInformationMessage =
        "I do not have enough approved information to answer that question.";
    private const string UnexpectedResponseMessage =
        "AzureFoundry answer generation returned an unexpected response shape.";

    private readonly AzureFoundryOptions _options;
    private readonly GroundedPromptBuilder _promptBuilder;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AzureFoundryAnswerService> _logger;

    public AzureFoundryAnswerService(
        IOptions<AzureFoundryOptions> options,
        GroundedPromptBuilder promptBuilder,
        HttpClient httpClient,
        ILogger<AzureFoundryAnswerService> logger)
    {
        _options = options.Value;
        _promptBuilder = promptBuilder;
        _httpClient = httpClient;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_options.Endpoint) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.DeploymentName))
        {
            _logger.LogError(MissingConfigurationMessage);
            throw new InvalidOperationException(MissingConfigurationMessage);
        }
    }

    public async Task<AnswerDraft> GenerateAnswerAsync(
        string question,
        IReadOnlyList<SearchDocument> documents,
        CancellationToken cancellationToken)
    {
        // Azure AI Search retrieves candidate documents; TrustCortex policy filtering
        // happens before this service receives only approved context.
        if (documents.Count == 0)
        {
            return new AnswerDraft(InsufficientApprovedInformationMessage, []);
        }

        // AzureFoundry generates an answer only from approved documents passed here.
        var prompt = _promptBuilder.Build(question, documents);
        var endpoint = BuildChatCompletionsEndpoint();
        var requestBody = new
        {
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            max_tokens = _options.MaxTokens,
            temperature = _options.Temperature
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Add("api-key", _options.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "AzureFoundry answer generation failed with status {StatusCode}.",
                (int)response.StatusCode);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "AzureFoundry error. Status={StatusCode}. Body={Body}",
                (int)response.StatusCode,
                body);

            throw new InvalidOperationException(
                $"AzureFoundry answer generation failed. Status={(int)response.StatusCode}. Body={body}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var generatedAnswer = ExtractAnswer(responseContent);
        var citations = documents
            .Select(document => new CitationDto(document.Id, document.Title, document.Content))
            .ToArray();

        return new AnswerDraft(generatedAnswer, citations);
    }

    private Uri BuildChatCompletionsEndpoint()
    {
        var baseEndpoint = _options.Endpoint.TrimEnd('/');
        var deploymentName = Uri.EscapeDataString(_options.DeploymentName);
        var apiVersion = Uri.EscapeDataString(
            string.IsNullOrWhiteSpace(_options.ApiVersion)
                ? "2024-10-21"
                : _options.ApiVersion);

        var endpoint =
            $"{baseEndpoint}/openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                "AzureFoundry answer provider is selected, but Endpoint is not a valid absolute URI.");
        }

        return uri;
    }

    private static string ExtractAnswer(string responseContent)
    {
        try
        {
            using var document = JsonDocument.Parse(responseContent);

            if (!document.RootElement.TryGetProperty("choices", out var choices) ||
                choices.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException(UnexpectedResponseMessage);
            }

            if (choices.GetArrayLength() == 0)
            {
                return InsufficientApprovedInformationMessage;
            }

            var firstChoice = choices[0];
            if (!firstChoice.TryGetProperty("message", out var message) ||
                !message.TryGetProperty("content", out var content) ||
                content.ValueKind != JsonValueKind.String)
            {
                throw new InvalidOperationException(UnexpectedResponseMessage);
            }

            return content.GetString() ?? InsufficientApprovedInformationMessage;
        }
        catch (JsonException)
        {
            throw new InvalidOperationException(UnexpectedResponseMessage);
        }
    }
}
