using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Application.Governance;

public sealed class PolicyEngine : IPolicyEngine
{
    private static readonly IReadOnlyDictionary<string, string[]> AllowedSensitivities =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Engineer"] = ["Public", "Internal"],
            ["Manager"] = ["Public", "Internal", "Confidential"],
            ["ComplianceOfficer"] = ["Public", "Internal", "Confidential", "Restricted"]
        };

    public PolicyEvaluationResult Evaluate(string userRole, IReadOnlyList<SearchDocument> documents)
    {
        var allowed = AllowedSensitivities.TryGetValue(userRole, out var sensitivities)
            ? sensitivities
            : Array.Empty<string>();

        var allowedDocuments = documents
            .Where(document => allowed.Contains(document.Sensitivity, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        var blockedCount = documents.Count - allowedDocuments.Length;

        return new PolicyEvaluationResult(
            Passed: blockedCount == 0,
            AllowedDocuments: allowedDocuments,
            DocumentsBlocked: blockedCount,
            BlockedReason: blockedCount > 0 ? "RestrictedSensitivity" : null);
    }
}
