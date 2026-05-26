namespace Syncra.Worker3;

public static class DependencyInjection
{
    public static IServiceCollection AddWorker3Services(this IServiceCollection services)
    {
        services.AddSingleton<IWorker3, Worker>();
        return services;
    }
}