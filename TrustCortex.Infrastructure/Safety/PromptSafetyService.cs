using TrustCortex.Application.Governance;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Infrastructure.Safety;

public sealed class PromptSafetyService : IPromptSafetyService
{
    private static readonly string[] BlockedPhrases =
    [
        "ignore previous instructions",
        "reveal system prompt",
        "bypass policy",
        "dump all documents",
        "show restricted data"
    ];

    public Task<PromptSafetyResult> EvaluateAsync(string prompt, CancellationToken cancellationToken)
    {
        var blockedPhrase = BlockedPhrases.FirstOrDefault(
            phrase => prompt.Contains(phrase, StringComparison.OrdinalIgnoreCase));

        var result = blockedPhrase is null
            ? new PromptSafetyResult(true, null)
            : new PromptSafetyResult(false, "PromptSafetyViolation");

        return Task.FromResult(result);
    }
}
