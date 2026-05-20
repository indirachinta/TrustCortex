namespace TrustCortex.Application.DTOs;

public sealed record AskRequest(
    string Question,
    string UserRole);
