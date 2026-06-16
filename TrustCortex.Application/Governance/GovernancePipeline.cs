using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Application.Governance;

public sealed class GovernancePipeline(
    IPromptSafetyService promptSafetyService,
    ISearchService searchService,
    IPurviewMetadataProvider purviewMetadataProvider,
    IPolicyEngine policyEngine)
{
    public async Task<GovernancePipelineResult> RunAsync(AskRequest request, CancellationToken cancellationToken)
    {
        var promptSafety = await promptSafetyService.EvaluateAsync(request.Question, cancellationToken);
        if (!promptSafety.Passed)
        {
            return new GovernancePipelineResult(
                PromptSafetyPassed: false,
                PolicyEvaluation: new PolicyEvaluationResult(
                    Passed: false,
                    AllowedDocuments: [],
                    DocumentsRetrieved: 0,
                    DocumentsApproved: 0,
                    DocumentsBlocked: 0,
                    BlockedReason: promptSafety.BlockedReason,
                    GovernanceMetadata: []));
        }

        var documents = await searchService.SearchAsync(request.Question, cancellationToken);
        var metadataByDocumentId = await ResolveMetadataAsync(documents, cancellationToken);
        var policyEvaluation = policyEngine.Evaluate(request.UserRole, documents, metadataByDocumentId);

        return new GovernancePipelineResult(
            PromptSafetyPassed: true,
            PolicyEvaluation: policyEvaluation);
    }

    private async Task<IReadOnlyDictionary<string, GovernanceMetadata>> ResolveMetadataAsync(
        IReadOnlyList<SearchDocument> documents,
        CancellationToken cancellationToken)
    {
        var metadataByDocumentId = new Dictionary<string, GovernanceMetadata>(StringComparer.OrdinalIgnoreCase);

        foreach (var document in documents)
        {
            var metadata = await purviewMetadataProvider.GetMetadataAsync(document.Id, cancellationToken);
            if (metadata is not null)
            {
                metadataByDocumentId[document.Id] = metadata;
            }
        }

        return metadataByDocumentId;
    }
}

public sealed record GovernancePipelineResult(
    bool PromptSafetyPassed,
    PolicyEvaluationResult PolicyEvaluation);
