namespace TrustCortex.Infrastructure.Search;

public class AzureSearchOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AdminKey { get; set; } = string.Empty;
    public string IndexName { get; set; } = "trustcortex-documents";
}