using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrustCortex.Api.Controllers;
using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;
using TrustCortex.Infrastructure;
using TrustCortex.Infrastructure.Answers;
using TrustCortex.Infrastructure.Search;

namespace TrustCortex.Tests;

public sealed class V4RuntimeSafetyTests
{
    [Fact]
    public async Task MockMode_DoesNotRequireAzureFoundryConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SearchProvider"] = "Mock",
                ["AnswerProvider"] = "Mock",
                ["AzureFoundry:Endpoint"] = string.Empty,
                ["AzureFoundry:ApiKey"] = string.Empty,
                ["AzureFoundry:DeploymentName"] = string.Empty
            })
            .Build();
        var services = new ServiceCollection();

        services.AddTrustCortexInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        var answerService = provider.GetRequiredService<IAnswerService>();
        var answer = await answerService.GenerateAnswerAsync(
            "Can customer PII be logged in App Insights?",
            [
                new SearchDocument(
                    "doc-internal",
                    "Application Insights PII Logging Policy",
                    "Customer PII must not be logged in Application Insights.",
                    "Internal",
                    "security-policy.pdf",
                    "Engineer,Manager,ComplianceOfficer")
            ],
            CancellationToken.None);

        Assert.IsType<MockAnswerService>(answerService);
        Assert.NotEmpty(answer.Answer);
        Assert.NotEmpty(answer.Citations);
    }

    [Fact]
    public void GroundedPromptBuilder_DoesNotIncludeBlockedDocuments()
    {
        var builder = new GroundedPromptBuilder();
        var approvedDocuments = new[]
        {
            new SearchDocument(
                "doc-public",
                "Public Telemetry Overview",
                "Approved public telemetry guidance.",
                "Public",
                "public-telemetry.md",
                "Engineer,Manager,ComplianceOfficer"),
            new SearchDocument(
                "doc-internal",
                "Application Insights PII Logging Policy",
                "Customer PII must not be logged in Application Insights.",
                "Internal",
                "security-policy.pdf",
                "Engineer,Manager,ComplianceOfficer")
        };
        var blockedDocument = new SearchDocument(
            "doc-restricted",
            "Restricted Payroll Incident Report",
            "Restricted payroll incident report involving employee compensation data.",
            "Restricted",
            "payroll-incident.pdf",
            "ComplianceOfficer");

        var prompt = builder.Build(
            "Can customer PII be logged in App Insights?",
            approvedDocuments);

        Assert.Contains("Answer only using approved context.", prompt);
        Assert.Contains("Do not mention blocked or restricted documents that are not present in approved context.", prompt);
        Assert.Contains("Cite sources from approved context.", prompt);
        Assert.Contains("Public Telemetry Overview", prompt);
        Assert.Contains("Approved public telemetry guidance.", prompt);
        Assert.Contains("Sensitivity level: Public", prompt);
        Assert.Contains("Application Insights PII Logging Policy", prompt);
        Assert.Contains("Customer PII must not be logged in Application Insights.", prompt);
        Assert.DoesNotContain(blockedDocument.Title, prompt);
        Assert.DoesNotContain(blockedDocument.Content, prompt);
    }

    [Fact]
    public async Task AzureFoundryAnswerService_EmptyApprovedContext_DoesNotCallAzure()
    {
        var httpClient = new HttpClient(new ThrowingHttpMessageHandler());
        var service = new AzureFoundryAnswerService(
            Options.Create(new AzureFoundryOptions
            {
                Endpoint = "https://example.openai.azure.com",
                ApiKey = "test-key",
                DeploymentName = "test-deployment"
            }),
            new GroundedPromptBuilder(),
            httpClient,
            NullLogger<AzureFoundryAnswerService>.Instance);

        var answer = await service.GenerateAnswerAsync(
            "Can customer PII be logged in App Insights?",
            [],
            CancellationToken.None);

        Assert.Equal("I do not have enough approved information to answer that question.", answer.Answer);
        Assert.Empty(answer.Citations);
    }

    [Fact]
    public async Task AzureFoundryAnswerService_UsesOnlyApprovedDocumentsForPromptAndCitations()
    {
        var requestContent = string.Empty;
        var httpClient = new HttpClient(new StubHttpMessageHandler(async request =>
        {
            requestContent = await request.Content!.ReadAsStringAsync();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "choices": [
                        {
                          "message": {
                            "content": "Customer PII must not be logged in Application Insights."
                          }
                        }
                      ]
                    }
                    """)
            };
        }));
        var service = new AzureFoundryAnswerService(
            Options.Create(new AzureFoundryOptions
            {
                Endpoint = "https://example.openai.azure.com",
                ApiKey = "test-key",
                DeploymentName = "test-deployment"
            }),
            new GroundedPromptBuilder(),
            httpClient,
            NullLogger<AzureFoundryAnswerService>.Instance);

        var answer = await service.GenerateAnswerAsync(
            "Can customer PII be logged in App Insights?",
            [
                new SearchDocument(
                    "doc-internal",
                    "Application Insights PII Logging Policy",
                    "Customer PII must not be logged in Application Insights.",
                    "Internal",
                    "security-policy.pdf",
                    "Engineer,Manager,ComplianceOfficer")
            ],
            CancellationToken.None);

        Assert.Equal("Customer PII must not be logged in Application Insights.", answer.Answer);
        var citation = Assert.Single(answer.Citations);
        Assert.Equal("doc-internal", citation.DocumentId);
        Assert.Equal("Application Insights PII Logging Policy", citation.Title);
        Assert.Contains("Application Insights PII Logging Policy", requestContent);
        Assert.Contains("Customer PII must not be logged in Application Insights.", requestContent);
        Assert.DoesNotContain("Restricted Payroll Incident Report", requestContent);
        Assert.DoesNotContain("employee compensation data", requestContent);
    }

    [Fact]
    public async Task AzureFoundryAnswerService_UnexpectedJson_ReturnsControlledError()
    {
        var httpClient = new HttpClient(new StubHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "unexpected": true }""")
            })));
        var service = new AzureFoundryAnswerService(
            Options.Create(new AzureFoundryOptions
            {
                Endpoint = "https://example.openai.azure.com",
                ApiKey = "test-key",
                DeploymentName = "test-deployment"
            }),
            new GroundedPromptBuilder(),
            httpClient,
            NullLogger<AzureFoundryAnswerService>.Instance);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GenerateAnswerAsync(
                "Can customer PII be logged in App Insights?",
                [
                    new SearchDocument(
                        "doc-internal",
                        "Application Insights PII Logging Policy",
                        "Customer PII must not be logged in Application Insights.",
                        "Internal",
                        "security-policy.pdf",
                        "Engineer,Manager,ComplianceOfficer")
                ],
                CancellationToken.None));

        Assert.Equal("AzureFoundry answer generation returned an unexpected response shape.", exception.Message);
        Assert.DoesNotContain("test-key", exception.ToString());
        Assert.DoesNotContain("https://example.openai.azure.com", exception.ToString());
        Assert.DoesNotContain("Can customer PII be logged", exception.ToString());
    }

    [Fact]
    public void RuntimeStatus_DoesNotExposeSecrets()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SearchProvider"] = "Azure",
                ["AnswerProvider"] = "AzureFoundry",
                ["AzureSearch:Endpoint"] = "https://search.example.net",
                ["AzureSearch:AdminKey"] = "super-secret-search-key",
                ["AzureSearch:IndexName"] = "trustcortex-documents",
                ["AzureFoundry:Endpoint"] = "https://foundry.example.net",
                ["AzureFoundry:ApiKey"] = "super-secret-foundry-key",
                ["AzureFoundry:DeploymentName"] = "gpt-demo"
            })
            .Build();
        var controller = new AdminController(new NoOpSearchIndexInitializer(), configuration);

        var result = Assert.IsType<OkObjectResult>(controller.GetRuntimeStatus());
        var json = JsonSerializer.Serialize(result.Value);

        Assert.Contains("\"searchProvider\":\"Azure\"", json);
        Assert.Contains("\"answerProvider\":\"AzureFoundry\"", json);
        Assert.Contains("configured", json);
        Assert.Contains("true", json);
        Assert.DoesNotContain("ApiKey", json);
        Assert.DoesNotContain("AdminKey", json);
        Assert.DoesNotContain("super-secret-search-key", json);
        Assert.DoesNotContain("super-secret-foundry-key", json);
        Assert.DoesNotContain("https://search.example.net", json);
        Assert.DoesNotContain("https://foundry.example.net", json);
        Assert.DoesNotContain("gpt-demo", json);
    }

    private sealed class ThrowingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("HTTP should not be called for empty approved context.");
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handler(request);
        }
    }

    private sealed class NoOpSearchIndexInitializer : ISearchIndexInitializer
    {
        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
