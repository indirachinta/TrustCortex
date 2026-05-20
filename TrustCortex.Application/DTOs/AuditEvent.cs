namespace TrustCortex.Application.DTOs;

public sealed record AuditEvent(
    string Question,
    string UserRole,
    bool PolicyCheckPassed,
    bool PromptSafetyPassed,
    int DocumentsBlocked,
    string? BlockedReason,
    bool ResponseGrounded);
