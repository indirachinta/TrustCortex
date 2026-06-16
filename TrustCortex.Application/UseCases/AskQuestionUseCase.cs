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
                validation.IsGrounded,
                governance.PolicyEvaluation.GovernanceMetadata),
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
                AuditLogged: auditLogged,
                ClassificationSource: GetClassificationSource(governance.PolicyEvaluation.GovernanceMetadata),
                EvaluatedClassification: GetEvaluatedClassification(governance.PolicyEvaluation.GovernanceMetadata)));
    }

    private static string? GetClassificationSource(
        IReadOnlyList<AuditGovernanceMetadata> governanceMetadata)
    {
        return governanceMetadata
            .Select(metadata => metadata.SourceSystem)
            .FirstOrDefault(sourceSystem =>
                !string.IsNullOrWhiteSpace(sourceSystem) &&
                !string.Equals(sourceSystem, "Missing", StringComparison.OrdinalIgnoreCase));
    }

    private static string? GetEvaluatedClassification(
        IReadOnlyList<AuditGovernanceMetadata> governanceMetadata)
    {
        return governanceMetadata
            .Select(metadata => metadata.Classification)
            .Where(classification =>
                !string.IsNullOrWhiteSpace(classification) &&
                !string.Equals(classification, "Missing", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(GetClassificationRank)
            .FirstOrDefault();
    }

    private static int GetClassificationRank(string classification)
    {
        return classification switch
        {
            "HighlyConfidential" => 4,
            "Confidential" => 3,
            "Internal" => 2,
            "Public" => 1,
            _ => 0
        };
    }
}
