namespace TrustCortex.Application.DTOs;

public sealed record GovernanceMetadataDto(
    bool PromptSafetyPassed,
    bool PolicyCheckPassed,
    int DocumentsRetrieved,
    int DocumentsApproved,
    int DocumentsBlocked,
    string? BlockedReason,
    bool ResponseGrounded,
    bool AuditLogged,
    string? ClassificationSource = null,
    string? EvaluatedClassification = null);
