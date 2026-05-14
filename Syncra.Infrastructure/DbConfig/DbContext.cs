using Microsoft.EntityFrameworkCore;

namespace Syncra.Infrastructure;

public class SyncraDbContext(DbContextOptions<SyncraDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SyncraDbContext).Assembly);
    }
}