using Microsoft.EntityFrameworkCore;
using Syncra.Domain.Entities;

namespace Syncra.Infrastructure;

public class SyncraDbContext(DbContextOptions<SyncraDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<AccountSnapshot> AccountSnapshots { get; set; } = null!;
    public DbSet<AccountState> AccountStates { get; set; } = null!;
    public DbSet<Conflict> Conflicts { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventArchive> EventArchives { get; set; } = null!;
    public DbSet<NodeState> NodeStates { get; set; } = null!;
    public DbSet<IdempotencyKey> IdempotencyKeys { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SyncraDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountCofiguration).Assembly);
    }
}