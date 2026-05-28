using System.ComponentModel.DataAnnotations;

namespace TrustCortex.Application.DTOs;

public sealed record AskRequest(
    [Required]
    string Question,
    [Required]
    string UserRole);
