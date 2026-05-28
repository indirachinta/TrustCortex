using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;
using TrustCortex.Infrastructure.Mocks;

namespace TrustCortex.Infrastructure.Search;

public class MockSearchService : ISearchService
{
    private readonly SampleDocumentLoader _documentLoader;

    public MockSearchService(SampleDocumentLoader documentLoader)
    {
        _documentLoader = documentLoader;
    }

    public async Task<IReadOnlyList<SearchDocument>> SearchAsync(
        string question,
        CancellationToken cancellationToken = default)
    {
        var documents = await _documentLoader.LoadAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(question))
        {
            return documents;
        }

        var keywords = question
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => x.Length > 2)
            .Select(x => x.ToLowerInvariant())
            .ToList();

        return documents
            .Where(doc =>
                keywords.Any(keyword =>
                    doc.Title.ToLowerInvariant().Contains(keyword) ||
                    doc.Content.ToLowerInvariant().Contains(keyword)))
            .Take(5)
            .ToList();
    }
}