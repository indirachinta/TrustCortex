using System.Text.Json;
using System.Text.Json.Serialization;
using TrustCortex.Application.Governance;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Purview;

public sealed class MockPurviewMetadataProvider : IPurviewMetadataProvider
{
    private readonly string _filePath;

    public MockPurviewMetadataProvider()
    {
        _filePath = Path.Combine(
            AppContext.BaseDirectory,
            "sample-data",
            "purview-metadata.json");
    }

    public async Task<GovernanceMetadata?> GetMetadataAsync(
        string documentId,
        CancellationToken cancellationToken)
    {
        var metadata = File.Exists(_filePath)
            ? await LoadFromFileAsync(cancellationToken)
            : GetFallbackMetadata();

        return metadata.FirstOrDefault(item =>
            string.Equals(item.DocumentId, documentId, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IReadOnlyList<GovernanceMetadata>> LoadFromFileAsync(
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(_filePath);

        var metadata = await JsonSerializer.DeserializeAsync<List<GovernanceMetadata>>(
            stream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            },
            cancellationToken);

        return metadata ?? [];
    }

    private static IReadOnlyList<GovernanceMetadata> GetFallbackMetadata()
    {
        return
        [
            new GovernanceMetadata
            {
                DocumentId = "doc-001",
                Classification = GovernanceClassification.Internal,
                SourceSystem = "Purview",
                OwnerDepartment = "Security",
                RetentionPolicy = "Standard-5Years",
                LastReviewedDate = new DateOnly(2026, 6, 1)
            },
            new GovernanceMetadata
            {
                DocumentId = "doc-002",
                Classification = GovernanceClassification.HighlyConfidential,
                SourceSystem = "Purview",
                OwnerDepartment = "Compliance",
                RetentionPolicy = "Restricted-7Years",
                LastReviewedDate = new DateOnly(2026, 6, 1)
            }
        ];
    }
}
