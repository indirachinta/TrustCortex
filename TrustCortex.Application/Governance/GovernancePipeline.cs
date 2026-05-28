using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Application.Governance;

public sealed class GovernancePipeline(
    IPromptSafetyService promptSafetyService,
    ISearchService searchService,
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
                    BlockedReason: promptSafety.BlockedReason));
        }

        var documents = await searchService.SearchAsync(request.Question, cancellationToken);
        var policyEvaluation = policyEngine.Evaluate(request.UserRole, documents);

        return new GovernancePipelineResult(
            PromptSafetyPassed: true,
            PolicyEvaluation: policyEvaluation);
    }
}

public sealed record GovernancePipelineResult(
    bool PromptSafetyPassed,
    PolicyEvaluationResult PolicyEvaluation);
