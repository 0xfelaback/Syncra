using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Syncra.Infrastructure;

public class SyncraDbContextFactory : IDesignTimeDbContextFactory<SyncraDbContext>
{
    public SyncraDbContext CreateDbContext(string[] args)
    {
        var projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Syncra.Api"));
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
        string connString = configuration.GetConnectionString("localConnectionString")!;
        var optionsBuilder = new DbContextOptionsBuilder<SyncraDbContext>();
        optionsBuilder.UseNpgsql(connString);

        return new SyncraDbContext(optionsBuilder.Options);
    }
}