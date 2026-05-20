namespace TrustCortex.Application.Governance;

public sealed record PromptSafetyResult(
    bool Passed,
    string? BlockedReason);
