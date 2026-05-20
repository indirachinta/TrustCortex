using Microsoft.Extensions.DependencyInjection;
using TrustCortex.Application.Governance;
using TrustCortex.Application.Interfaces;
using TrustCortex.Application.UseCases;
using TrustCortex.Application.Validation;

namespace TrustCortex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTrustCortexApplication(this IServiceCollection services)
    {
        services.AddScoped<IPolicyEngine, PolicyEngine>();
        services.AddScoped<IResponseValidator, ResponseValidator>();
        services.AddScoped<GovernancePipeline>();
        services.AddScoped<AskQuestionUseCase>();

        return services;
    }
}
