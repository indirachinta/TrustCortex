using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Options;
using TrustCortex.Application.Interfaces;
using TrustCortex.Infrastructure.Mocks;

namespace TrustCortex.Infrastructure.Search;

public class AzureSearchIndexInitializer : ISearchIndexInitializer
{
    private readonly AzureSearchOptions _options;
    private readonly SampleDocumentLoader _documentLoader;

    public AzureSearchIndexInitializer(
        IOptions<AzureSearchOptions> options,
        SampleDocumentLoader documentLoader)
    {
        _options = options.Value;
        _documentLoader = documentLoader;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        ValidateOptions();

        var credential = new AzureKeyCredential(_options.AdminKey);

        var indexClient = new SearchIndexClient(
            new Uri(_options.Endpoint),
            credential);

        var searchClient = new SearchClient(
            new Uri(_options.Endpoint),
            _options.IndexName,
            credential);

        await CreateIndexIfNotExistsAsync(indexClient, cancellationToken);
        await UploadSampleDocumentsAsync(searchClient, cancellationToken);
    }

    private async Task CreateIndexIfNotExistsAsync(
        SearchIndexClient indexClient,
        CancellationToken cancellationToken)
    {
        var fields = new FieldBuilder().Build(typeof(AzureSearchDocument));

        var index = new SearchIndex(_options.IndexName, fields);

        try
        {
            await indexClient.GetIndexAsync(_options.IndexName, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            await indexClient.CreateIndexAsync(index, cancellationToken);
        }
    }

    private async Task UploadSampleDocumentsAsync(
        SearchClient searchClient,
        CancellationToken cancellationToken)
    {
        var documents = await _documentLoader.LoadAsync(cancellationToken);

        var azureDocuments = documents.Select(doc => new AzureSearchDocument
        {
            Id = doc.Id,
            Title = doc.Title,
            Content = doc.Content,
            Source = doc.Source,
            SensitivityLevel = doc.Sensitivity,
            AllowedRoles = doc.AllowedRoles
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        });

        await searchClient.MergeOrUploadDocumentsAsync(
            azureDocuments,
            cancellationToken: cancellationToken);
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.Endpoint))
        {
            throw new InvalidOperationException("AzureSearch:Endpoint is missing.");
        }

        if (string.IsNullOrWhiteSpace(_options.AdminKey))
        {
            throw new InvalidOperationException("AzureSearch:AdminKey is missing.");
        }

        if (string.IsNullOrWhiteSpace(_options.IndexName))
        {
            throw new InvalidOperationException("AzureSearch:IndexName is missing.");
        }
    }
}
