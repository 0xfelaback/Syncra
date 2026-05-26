namespace Syncra.Worker2;

public static class DependencyInjection
{
    public static IServiceCollection AddWorker2Services(this IServiceCollection services)
    {
        services.AddSingleton<IWorker2, Worker>();
        return services;
    }
}