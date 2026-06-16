namespace TrustCortex.Application.DTOs;

public sealed record AuditGovernanceMetadata(
    string DocumentId,
    string Classification,
    string SourceSystem,
    string PolicyDecision);
