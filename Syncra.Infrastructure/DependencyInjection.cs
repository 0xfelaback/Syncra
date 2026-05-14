using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Syncra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, string connString)
    {
        services.AddDbContext<SyncraDbContext>(options => options.UseNpgsql(connString));
        return services;
    }
}