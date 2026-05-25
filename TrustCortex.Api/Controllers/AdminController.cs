using Microsoft.AspNetCore.Mvc;
using TrustCortex.Application.Interfaces;

namespace TrustCortex.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ISearchIndexInitializer _searchIndexInitializer;
    private readonly IConfiguration _configuration;

    public AdminController(
        ISearchIndexInitializer searchIndexInitializer,
        IConfiguration configuration)
    {
        _searchIndexInitializer = searchIndexInitializer;
        _configuration = configuration;
    }

    [HttpPost("search/initialize")]
    public async Task<IActionResult> InitializeSearchIndex(
        CancellationToken cancellationToken)
    {
        var provider = _configuration["SearchProvider"] ?? "Mock";

        await _searchIndexInitializer.InitializeAsync(cancellationToken);

        return Ok(new
        {
            message = provider.Equals("Azure", StringComparison.OrdinalIgnoreCase)
                ? "Azure AI Search index initialized and sample documents uploaded."
                : "Mock mode enabled. No Azure index initialization performed.",
            searchProvider = provider
        });
    }
}