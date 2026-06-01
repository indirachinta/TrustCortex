namespace TrustCortex.Infrastructure.Answers;

public sealed class AzureFoundryOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "2024-10-21";
    public int MaxTokens { get; set; } = 600;
    public double Temperature { get; set; } = 0.2;
}
