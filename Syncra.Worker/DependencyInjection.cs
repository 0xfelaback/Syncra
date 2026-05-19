using RabbitMQ.Client;

namespace Syncra.Worker;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory>(new ConnectionFactory { HostName = "localhost" });
        services.AddHostedService<EventSubscribeWorker>();
        return services;
    }
}