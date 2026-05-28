using System.Text.Json;
using TrustCortex.Application.DTOs;

namespace TrustCortex.Infrastructure.Mocks;

public class SampleDocumentLoader
{
    private readonly string _filePath;

    public SampleDocumentLoader()
    {
        _filePath = Path.Combine(
            AppContext.BaseDirectory,
            "sample-data",
            "enterprise-documents.json");
    }

    public async Task<IReadOnlyList<SearchDocument>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return GetFallbackDocuments();
        }

        await using var stream = File.OpenRead(_filePath);

        var documents = await JsonSerializer.DeserializeAsync<List<SampleSearchDocument>>(
            stream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            cancellationToken);

        return documents?.Select(document => document.ToSearchDocument()).ToArray()
            ?? GetFallbackDocuments();
    }

    private static IReadOnlyList<SearchDocument> GetFallbackDocuments()
    {
        return new List<SearchDocument>
        {
            new(
                "doc-001",
                "Application Insights PII Logging Policy",
                "Customer PII must not be logged in Application Insights.",
                "Internal",
                "security-policy.pdf",
                "Engineer,Manager,ComplianceOfficer"),
            new(
                "doc-002",
                "Restricted Customer Data Handling",
                "Restricted customer data handling details are available only to ComplianceOfficer users.",
                "Restricted",
                "compliance-manual.pdf",
                "ComplianceOfficer")
        };
    }

    private sealed record SampleSearchDocument(
        string Id,
        string Title,
        string Content,
        string Source,
        string SensitivityLevel,
        IReadOnlyList<string> AllowedRoles)
    {
        public SearchDocument ToSearchDocument()
        {
            return new SearchDocument(
                Id,
                Title,
                Content,
                SensitivityLevel,
                Source,
                string.Join(",", AllowedRoles));
        }
    }
}
