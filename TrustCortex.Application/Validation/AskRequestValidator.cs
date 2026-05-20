using TrustCortex.Application.DTOs;

namespace TrustCortex.Application.Validation;

public static class AskRequestValidator
{
    public static void Validate(AskRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ArgumentException("Question is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.UserRole))
        {
            throw new ArgumentException("UserRole is required.", nameof(request));
        }
    }
}
