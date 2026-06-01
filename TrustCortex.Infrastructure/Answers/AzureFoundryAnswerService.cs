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
        if (documents.Count == 0)
        {
            return new AnswerDraft(
                "I do not have enough approved information to answer that question.",
                []);
        }

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
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"AzureFoundry answer generation failed with status {(int)response.StatusCode}: {responseContent}");
        }

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

        // TODO: Confirm the target Azure Foundry/OpenAI API version before enabling this provider in production.
        return new Uri($"{baseEndpoint}/openai/deployments/{deploymentName}/chat/completions?api-version=2024-10-21");
    }

    private static string ExtractAnswer(string responseContent)
    {
        using var document = JsonDocument.Parse(responseContent);

        var choices = document.RootElement.GetProperty("choices");
        if (choices.GetArrayLength() == 0)
        {
            return "I do not have enough approved information to answer that question.";
        }

        return choices[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? "I do not have enough approved information to answer that question.";
    }
}
