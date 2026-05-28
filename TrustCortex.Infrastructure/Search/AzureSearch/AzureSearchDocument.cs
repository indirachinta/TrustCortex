using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace TrustCortex.Infrastructure.Search;

public class AzureSearchDocument
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = string.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Title { get; set; } = string.Empty;

    [SearchableField]
    public string Content { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public string Source { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public string SensitivityLevel { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    public string[] AllowedRoles { get; set; } = [];
}
