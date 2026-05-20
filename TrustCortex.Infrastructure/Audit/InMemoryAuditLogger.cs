using System.Collections.Concurrent;
using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Audit;

public sealed class InMemoryAuditLogger : IAuditLogger
{
    private readonly ConcurrentQueue<AuditEvent> _events = new();

    public Task<bool> LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        _events.Enqueue(auditEvent);
        return Task.FromResult(true);
    }
}
