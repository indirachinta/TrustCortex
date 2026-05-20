namespace TrustCortex.Application.DTOs;

public sealed record AnswerDraft(
    string Answer,
    IReadOnlyList<CitationDto> Citations);
