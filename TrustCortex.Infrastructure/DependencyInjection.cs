using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrustCortex.Application.Interfaces;
using TrustCortex.Infrastructure.Answers;
using TrustCortex.Infrastructure.Audit;
using TrustCortex.Infrastructure.Mocks;
using TrustCortex.Infrastructure.Safety;
using TrustCortex.Infrastructure.Search;

namespace TrustCortex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTrustCortexInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var searchProvider = configuration["SearchProvider"] ?? "Mock";
        services.AddSingleton<SampleDocumentLoader>();

        if (string.Equals(searchProvider, "Azure", StringComparison.OrdinalIgnoreCase))
        {
            services.Configure<AzureSearchOptions>(options =>
            {
                var section = configuration.GetSection("AzureSearch");
                options.Endpoint = section["Endpoint"] ?? string.Empty;
                options.AdminKey = section["AdminKey"] ?? string.Empty;
                options.IndexName = section["IndexName"] ?? "trustcortex-documents";
            });

            services.AddScoped<ISearchService, AzureAiSearchService>();
            services.AddScoped<ISearchIndexInitializer, AzureSearchIndexInitializer>();
        }
        else
        {
            services.AddScoped<ISearchService, MockSearchService>();
            services.AddScoped<ISearchIndexInitializer, MockSearchIndexInitializer>();
        }

        var answerProvider = configuration["AnswerProvider"] ?? "Mock";

        if (string.Equals(answerProvider, "AzureFoundry", StringComparison.OrdinalIgnoreCase))
        {
            services.Configure<AzureFoundryOptions>(options =>
            {
                var section = configuration.GetSection("AzureFoundry");
                options.Endpoint = section["Endpoint"] ?? string.Empty;
                options.ApiKey = section["ApiKey"] ?? string.Empty;
                options.DeploymentName = section["DeploymentName"] ?? string.Empty;
                options.ApiVersion = section["ApiVersion"] ?? "2024-10-21";
                options.MaxTokens = int.TryParse(section["MaxTokens"], out var maxTokens)
                    ? maxTokens
                    : 600;
                options.Temperature = double.TryParse(section["Temperature"], out var temperature)
                    ? temperature
                    : 0.2;
            });

            services.AddSingleton<HttpClient>();
            services.AddScoped<GroundedPromptBuilder>();
            services.AddScoped<IAnswerService, AzureFoundryAnswerService>();
        }
        else
        {
            services.AddScoped<IAnswerService, MockAnswerService>();
        }

        services.AddScoped<IPromptSafetyService, PromptSafetyService>();
        services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();

        return services;
    }
}
