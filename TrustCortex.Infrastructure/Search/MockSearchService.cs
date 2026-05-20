using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Search;

public sealed class MockSearchService : ISearchService
{
    public Task<IReadOnlyList<SearchDocument>> SearchAsync(string question, CancellationToken cancellationToken)
    {
        IReadOnlyList<SearchDocument> documents =
        [
            new SearchDocument(
                "doc-internal-001",
                "Application Insights Logging Standard",
                "Customer PII must not be logged in Application Insights. Use redaction before telemetry leaves the application boundary.",
                "Internal"),
            new SearchDocument(
                "doc-restricted-001",
                "Restricted Customer Data Handling",
                "Restricted customer data handling details are available only to ComplianceOfficer users.",
                "Restricted")
        ];

        return Task.FromResult(documents);
    }
}
