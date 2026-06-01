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
    public void GroundedPromptBuilder_UsesOnlyApprovedDocuments()
    {
        var builder = new GroundedPromptBuilder();
        var prompt = builder.Build(
            "Can customer PII be logged in App Insights?",
            [
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
            ]);

        Assert.Contains("Answer only using approved context.", prompt);
        Assert.Contains("Public Telemetry Overview", prompt);
        Assert.Contains("Approved public telemetry guidance.", prompt);
        Assert.Contains("Application Insights PII Logging Policy", prompt);
        Assert.Contains("Customer PII must not be logged in Application Insights.", prompt);
        Assert.DoesNotContain("Restricted Payroll Incident Report", prompt);
        Assert.DoesNotContain("Restricted payroll incident report involving employee compensation data.", prompt);
    }

    [Fact]
    public async Task AzureFoundryAnswerService_EmptyApprovedContext_ReturnsInsufficientInformation()
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

        Assert.Contains("configured", json);
        Assert.Contains("true", json);
        Assert.DoesNotContain("ApiKey", json);
        Assert.DoesNotContain("AdminKey", json);
        Assert.DoesNotContain("super-secret-search-key", json);
        Assert.DoesNotContain("super-secret-foundry-key", json);
        Assert.DoesNotContain("https://search.example.net", json);
        Assert.DoesNotContain("https://foundry.example.net", json);
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

    private sealed class NoOpSearchIndexInitializer : ISearchIndexInitializer
    {
        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
