namespace TrustCortex.Application.Interfaces;

public interface ISearchIndexInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}