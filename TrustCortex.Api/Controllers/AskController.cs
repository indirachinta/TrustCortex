using Microsoft.AspNetCore.Mvc;
using TrustCortex.Application.DTOs;
using TrustCortex.Application.UseCases;

namespace TrustCortex.Api.Controllers;

[ApiController]
[Route("api/ask")]
public sealed class AskController(AskQuestionUseCase askQuestionUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Ask(AskRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await askQuestionUseCase.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
