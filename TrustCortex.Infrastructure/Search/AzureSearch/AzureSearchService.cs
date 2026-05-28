using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;
using TrustCortex.Application.Interfaces;
using AppSearchDocument = TrustCortex.Application.DTOs.SearchDocument;


namespace TrustCortex.Infrastructure.Search;

public class AzureAiSearchService : ISearchService
{
    private readonly SearchClient _searchClient;

    public AzureAiSearchService(IOptions<AzureSearchOptions> options)
    {
        var config = options.Value;
        _searchClient = new SearchClient(
            new Uri(config.Endpoint),
            config.IndexName,
            new AzureKeyCredential(config.AdminKey));
    }

    public async Task<IReadOnlyList<AppSearchDocument>> SearchAsync(string question, CancellationToken cancellationToken)
    {
        var results = new List<AppSearchDocument>();

        var response = await _searchClient.SearchAsync<AzureSearchDocument>(
            question,
            new SearchOptions
            {
                Size = 5
            },
            cancellationToken);

        await foreach (var result in response.Value.GetResultsAsync().WithCancellation(cancellationToken))
        {
            results.Add(new AppSearchDocument(
                result.Document.Id,
                result.Document.Title,
                result.Document.Content,
                result.Document.SensitivityLevel,
                result.Document.Source,
                string.Join(",", result.Document.AllowedRoles)));
        }

        return results;
    }

   
}
