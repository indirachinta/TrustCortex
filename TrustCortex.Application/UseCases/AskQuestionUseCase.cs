using TrustCortex.Application.DTOs;
using TrustCortex.Application.Governance;
using TrustCortex.Application.Interfaces;
using TrustCortex.Application.Validation;

namespace TrustCortex.Application.UseCases;

public sealed class AskQuestionUseCase(
    GovernancePipeline governancePipeline,
    IAnswerService answerService,
    IResponseValidator responseValidator,
    IAuditLogger auditLogger)
{
    public async Task<AskResponse> ExecuteAsync(AskRequest request, CancellationToken cancellationToken)
    {
        AskRequestValidator.Validate(request);

        var governance = await governancePipeline.RunAsync(request, cancellationToken);
        var approvedDocuments = governance.PolicyEvaluation.AllowedDocuments;

        var answer = governance.PromptSafetyPassed
            ? await answerService.GenerateAnswerAsync(
                request.Question,
                approvedDocuments,
                cancellationToken)
            : new AnswerDraft("I cannot answer that request because it violates prompt safety rules.", []);

        var validation = responseValidator.Validate(answer, approvedDocuments);

        var auditLogged = await auditLogger.LogAsync(
            new AuditEvent(
                request.Question,
                request.UserRole,
                governance.PolicyEvaluation.Passed,
                governance.PromptSafetyPassed,
                governance.PolicyEvaluation.DocumentsRetrieved,
                governance.PolicyEvaluation.DocumentsApproved,
                governance.PolicyEvaluation.DocumentsBlocked,
                governance.PolicyEvaluation.BlockedReason,
                validation.IsGrounded),
            cancellationToken);

        return new AskResponse(
            answer.Answer,
            answer.Citations,
            new GovernanceMetadataDto(
                PromptSafetyPassed: governance.PromptSafetyPassed,
                PolicyCheckPassed: governance.PolicyEvaluation.Passed,
                DocumentsRetrieved: governance.PolicyEvaluation.DocumentsRetrieved,
                DocumentsApproved: governance.PolicyEvaluation.DocumentsApproved,
                DocumentsBlocked: governance.PolicyEvaluation.DocumentsBlocked,
                BlockedReason: governance.PolicyEvaluation.BlockedReason,
                ResponseGrounded: validation.IsGrounded,
                AuditLogged: auditLogged));
    }
}
