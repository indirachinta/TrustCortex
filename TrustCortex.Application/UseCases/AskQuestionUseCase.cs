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

        var answer = governance.PromptSafetyPassed
            ? await answerService.GenerateAnswerAsync(
                request.Question,
                governance.PolicyEvaluation.AllowedDocuments,
                cancellationToken)
            : new AnswerDraft("I cannot answer that request because it violates prompt safety rules.", []);

        var validation = responseValidator.Validate(answer, governance.PolicyEvaluation.AllowedDocuments);

        var auditLogged = await auditLogger.LogAsync(
            new AuditEvent(
                request.Question,
                request.UserRole,
                governance.PolicyEvaluation.Passed,
                governance.PromptSafetyPassed,
                governance.PolicyEvaluation.DocumentsBlocked,
                governance.PolicyEvaluation.BlockedReason,
                validation.IsGrounded),
            cancellationToken);

        return new AskResponse(
            answer.Answer,
            answer.Citations,
            new GovernanceMetadataDto(
                PolicyCheckPassed: governance.PolicyEvaluation.Passed,
                PromptSafetyPassed: governance.PromptSafetyPassed,
                DocumentsBlocked: governance.PolicyEvaluation.DocumentsBlocked,
                BlockedReason: governance.PolicyEvaluation.BlockedReason,
                ResponseGrounded: validation.IsGrounded,
                AuditLogged: auditLogged));
    }
}
