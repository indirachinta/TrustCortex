using TrustCortex.Application.DTOs;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Answers;

public sealed class MockAnswerService : IAnswerService
{
    public Task<AnswerDraft> GenerateAnswerAsync(
        string question,
        IReadOnlyList<SearchDocument> documents,
        CancellationToken cancellationToken)
    {
        if (documents.Count == 0)
        {
            return Task.FromResult(new AnswerDraft(
                "I do not have enough policy-approved source material to answer that question.",
                []));
        }

        var citations = documents
            .Select(document => new CitationDto(document.Id, document.Title, document.Content))
            .ToArray();

        var answer = "No. Customer PII should not be logged in Application Insights. Use redaction or approved telemetry patterns before data leaves the application boundary.";

        return Task.FromResult(new AnswerDraft(answer, citations));
    }
}
