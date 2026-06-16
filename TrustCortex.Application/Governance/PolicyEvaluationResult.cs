using TrustCortex.Application.DTOs;

namespace TrustCortex.Application.Governance;

public sealed record PolicyEvaluationResult(
    bool Passed,
    IReadOnlyList<SearchDocument> AllowedDocuments,
    int DocumentsRetrieved,
    int DocumentsApproved,
    int DocumentsBlocked,
    string? BlockedReason,
    IReadOnlyList<AuditGovernanceMetadata> GovernanceMetadata);
