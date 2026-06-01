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

    [HttpGet("runtime-status")]
    public IActionResult GetRuntimeStatus()
    {
        var searchProvider = _configuration["SearchProvider"] ?? "Mock";
        var answerProvider = _configuration["AnswerProvider"] ?? "Mock";
        var azureSearch = _configuration.GetSection("AzureSearch");
        var azureFoundry = _configuration.GetSection("AzureFoundry");
        var azureSearchIndexName = azureSearch["IndexName"] ?? "trustcortex-documents";
        var azureFoundryDeploymentName = azureFoundry["DeploymentName"] ?? string.Empty;

        return Ok(new
        {
            searchProvider,
            answerProvider,
            azureSearch = new
            {
                configured = HasValue(azureSearch["Endpoint"]) && HasValue(azureSearch["AdminKey"]),
                indexName = azureSearchIndexName
            },
            azureFoundry = new
            {
                configured =
                    HasValue(azureFoundry["Endpoint"]) &&
                    HasValue(azureFoundry["ApiKey"]) &&
                    HasValue(azureFoundryDeploymentName),
                deploymentName = azureFoundryDeploymentName
            },
            costSafety = new
            {
                mockSearchDefault = string.Equals(searchProvider, "Mock", StringComparison.OrdinalIgnoreCase),
                mockAnswerDefault = string.Equals(answerProvider, "Mock", StringComparison.OrdinalIgnoreCase)
            }
        });
    }

    private static bool HasValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
