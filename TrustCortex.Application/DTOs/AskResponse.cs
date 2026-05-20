namespace TrustCortex.Application.DTOs;

public sealed record AskResponse(
    string Answer,
    IReadOnlyList<CitationDto> Citations,
    GovernanceMetadataDto Governance);
