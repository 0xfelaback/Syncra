using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Syncra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory>(new ConnectionFactory { HostName = "localhost" });
        return services;
    }
}