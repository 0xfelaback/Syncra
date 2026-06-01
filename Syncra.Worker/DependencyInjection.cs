using Syncra.Application.Interfaces;

namespace Syncra.Worker;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddSingleton<IWorker, Worker>();
        services.AddScoped<IEventValidatorService, EventValidatorService>();
        return services;
    }
}