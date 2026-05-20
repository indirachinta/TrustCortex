using TrustCortex.Application.DTOs;

namespace TrustCortex.Application.Interfaces;

public interface ISearchService
{
    Task<IReadOnlyList<SearchDocument>> SearchAsync(string question, CancellationToken cancellationToken);
}
