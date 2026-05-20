using Microsoft.Extensions.DependencyInjection;
using TrustCortex.Application.Interfaces;
using TrustCortex.Infrastructure.Answers;
using TrustCortex.Infrastructure.Audit;
using TrustCortex.Infrastructure.Safety;
using TrustCortex.Infrastructure.Search;

namespace TrustCortex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTrustCortexInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISearchService, MockSearchService>();
        services.AddScoped<IAnswerService, MockAnswerService>();
        services.AddScoped<IPromptSafetyService, PromptSafetyService>();
        services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();

        return services;
    }
}
