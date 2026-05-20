using TrustCortex.Application.DTOs;

namespace TrustCortex.Application.Interfaces;

public interface IAuditLogger
{
    Task<bool> LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken);
}
