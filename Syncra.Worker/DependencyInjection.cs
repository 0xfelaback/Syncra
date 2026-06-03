using Syncra.Application.Interfaces;
using Syncra.Worker.Services;

namespace Syncra.Worker;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddSingleton<IWorker, Worker>();
        services.AddScoped<IEventValidatorService, EventValidatorService>();
        services.AddScoped<IResolutionService, ResolutionService>();
        return services;
    }
}