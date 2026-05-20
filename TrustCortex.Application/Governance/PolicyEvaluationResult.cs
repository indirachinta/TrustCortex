using TrustCortex.Application.DTOs;

namespace TrustCortex.Application.Governance;

public sealed record PolicyEvaluationResult(
    bool Passed,
    IReadOnlyList<SearchDocument> AllowedDocuments,
    int DocumentsBlocked,
    string? BlockedReason);
