using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Application.Governance;

public sealed class PolicyEngine : IPolicyEngine
{
    private static readonly IReadOnlyDictionary<string, GovernanceClassification[]> AllowedClassifications =
        new Dictionary<string, GovernanceClassification[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Engineer"] =
            [
                GovernanceClassification.Public,
                GovernanceClassification.Internal
            ],
            ["ComplianceOfficer"] =
            [
                GovernanceClassification.Public,
                GovernanceClassification.Internal,
                GovernanceClassification.Confidential,
                GovernanceClassification.HighlyConfidential
            ]
        };

    public PolicyEvaluationResult Evaluate(
        string userRole,
        IReadOnlyList<SearchDocument> documents,
        IReadOnlyDictionary<string, GovernanceMetadata> metadataByDocumentId)
    {
        var allowed = AllowedClassifications.TryGetValue(userRole, out var classifications)
            ? classifications
            : Array.Empty<GovernanceClassification>();

        var allowedDocuments = documents
            .Where(document =>
                metadataByDocumentId.TryGetValue(document.Id, out var metadata) &&
                allowed.Contains(metadata.Classification))
            .ToArray();
        var allowedDocumentIds = allowedDocuments
            .Select(document => document.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var governanceMetadata = documents
            .Select(document => ToAuditGovernanceMetadata(
                document,
                metadataByDocumentId,
                allowedDocumentIds))
            .ToArray();

        var blockedCount = documents.Count - allowedDocuments.Length;

        return new PolicyEvaluationResult(
            Passed: blockedCount == 0,
            AllowedDocuments: allowedDocuments,
            DocumentsRetrieved: documents.Count,
            DocumentsApproved: allowedDocuments.Length,
            DocumentsBlocked: blockedCount,
            BlockedReason: blockedCount > 0 ? "ClassificationPolicy" : null,
            GovernanceMetadata: governanceMetadata);
    }

    private static AuditGovernanceMetadata ToAuditGovernanceMetadata(
        SearchDocument document,
        IReadOnlyDictionary<string, GovernanceMetadata> metadataByDocumentId,
        ISet<string> allowedDocumentIds)
    {
        if (!metadataByDocumentId.TryGetValue(document.Id, out var metadata))
        {
            return new AuditGovernanceMetadata(
                document.Id,
                "Missing",
                "Missing",
                "Blocked");
        }

        return new AuditGovernanceMetadata(
            document.Id,
            metadata.Classification.ToString(),
            metadata.SourceSystem,
            allowedDocumentIds.Contains(document.Id) ? "Approved" : "Blocked");
    }
}
