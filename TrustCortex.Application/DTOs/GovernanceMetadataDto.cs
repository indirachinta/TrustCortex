namespace TrustCortex.Application.DTOs;

public sealed record GovernanceMetadataDto(
    bool PolicyCheckPassed,
    bool PromptSafetyPassed,
    int DocumentsBlocked,
    string? BlockedReason,
    bool ResponseGrounded,
    bool AuditLogged);
