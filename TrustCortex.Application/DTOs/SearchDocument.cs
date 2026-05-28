namespace TrustCortex.Application.DTOs;

public sealed record SearchDocument(
    string Id,
    string Title,
    string Content,
    string Sensitivity,
    string Source,
    string AllowedRoles);
