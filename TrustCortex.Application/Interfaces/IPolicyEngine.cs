using TrustCortex.Application.DTOs;
using TrustCortex.Application.Governance;

namespace TrustCortex.Application.Interfaces;

public interface IPolicyEngine
{
    PolicyEvaluationResult Evaluate(string userRole, IReadOnlyList<SearchDocument> documents);
}
