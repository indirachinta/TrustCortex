using TrustCortex.Application.DTOs;
using TrustCortex.Application.Validation;

namespace TrustCortex.Application.Interfaces;

public interface IResponseValidator
{
    ResponseValidationResult Validate(AnswerDraft answer, IReadOnlyList<SearchDocument> sourceDocuments);
}
