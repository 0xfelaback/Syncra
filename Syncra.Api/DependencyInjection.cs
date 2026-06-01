using FluentValidation;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<DbUpdateConcurrencyExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHostedService<DataCleanupService>();
        services.AddScoped<IEntryService, EntryService>();
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}