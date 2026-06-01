using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class ConcurrencyTokenInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateConcurrencyTokens(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateConcurrencyTokens(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateConcurrencyTokens(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();
            var versionProperty = entityType.GetProperty("Version");
            var accountVersionProperty = entityType.GetProperty("AccountVersion");

            if (versionProperty != null && versionProperty.CanWrite)
            {
                versionProperty.SetValue(entry.Entity, Guid.NewGuid());
            }
            else if (accountVersionProperty != null && accountVersionProperty.CanWrite)
            {
                accountVersionProperty.SetValue(entry.Entity, Guid.NewGuid());
            }
        }
    }
}
