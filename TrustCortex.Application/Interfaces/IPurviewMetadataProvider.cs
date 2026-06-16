using TrustCortex.Application.Governance;

namespace TrustCortex.Application.Interfaces;

public interface IPurviewMetadataProvider
{
    Task<GovernanceMetadata?> GetMetadataAsync(
        string documentId,
        CancellationToken cancellationToken);
}
