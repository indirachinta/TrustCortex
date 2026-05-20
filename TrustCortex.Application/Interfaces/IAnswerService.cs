using TrustCortex.Application.DTOs;

namespace TrustCortex.Application.Interfaces;

public interface IAnswerService
{
    Task<AnswerDraft> GenerateAnswerAsync(string question, IReadOnlyList<SearchDocument> documents, CancellationToken cancellationToken);
}
