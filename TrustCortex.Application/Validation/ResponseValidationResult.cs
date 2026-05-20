namespace TrustCortex.Application.Validation;

public sealed record ResponseValidationResult(
    bool IsGrounded,
    string? Reason);
