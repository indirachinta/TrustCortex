namespace TrustCortex.Application.DTOs;

public sealed record CitationDto(
    string DocumentId,
    string Title,
    string Excerpt);
