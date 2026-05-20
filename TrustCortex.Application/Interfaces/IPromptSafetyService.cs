using TrustCortex.Application.Governance;

namespace TrustCortex.Application.Interfaces;

public interface IPromptSafetyService
{
    Task<PromptSafetyResult> EvaluateAsync(string prompt, CancellationToken cancellationToken);
}
