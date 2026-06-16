namespace TrustCortex.Application.Governance;

public sealed class GovernanceMetadata
{
    public required string DocumentId { get; init; }
    public required GovernanceClassification Classification { get; init; }
    public required string SourceSystem { get; init; }
    public required string OwnerDepartment { get; init; }
    public required string RetentionPolicy { get; init; }
    public required DateOnly LastReviewedDate { get; init; }
}
