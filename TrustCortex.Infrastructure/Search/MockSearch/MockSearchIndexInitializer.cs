using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Search;

public class MockSearchIndexInitializer : ISearchIndexInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}