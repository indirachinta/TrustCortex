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

        services.AddScoped<IAnswerService, MockAnswerService>();
        services.AddScoped<IPromptSafetyService, PromptSafetyService>();
        services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();

        return services;
    }
}
