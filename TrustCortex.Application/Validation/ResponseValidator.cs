using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Application.Validation;

public sealed class ResponseValidator : IResponseValidator
{
    public ResponseValidationResult Validate(AnswerDraft answer, IReadOnlyList<SearchDocument> sourceDocuments)
    {
        if (sourceDocuments.Count == 0)
        {
            return new ResponseValidationResult(answer.Citations.Count == 0, "No source documents were available.");
        }

        var sourceIds = sourceDocuments.Select(document => document.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var allCitationsGrounded = answer.Citations.All(citation => sourceIds.Contains(citation.DocumentId));

        return new ResponseValidationResult(
            IsGrounded: answer.Citations.Count > 0 && allCitationsGrounded,
            Reason: allCitationsGrounded ? null : "One or more citations were not present in the retrieved documents.");
    }
}
