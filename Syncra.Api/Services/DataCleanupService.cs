using Microsoft.EntityFrameworkCore;
using Syncra.Infrastructure;

public class DataCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _period = TimeSpan.FromHours(48);
    private readonly ILogger<DataCleanupService> _logger;
    public DataCleanupService(IServiceProvider serviceProvider, ILogger<DataCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new(_period);


        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Starting database cleanup job on Idempotency Keys table.");

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SyncraDbContext>();

                var cutoff = DateTime.UtcNow.AddHours(-48);

                int deletedRows = await dbContext.IdempotencyKeys
                    .Where(t => t.createdAt < cutoff).ExecuteDeleteAsync(cancellationToken);

                _logger.LogInformation("Cleanup finished. Deleted {Count} rows.", deletedRows);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while cleaning up the database: couldn't past Idempotency Keys");
            }
        }
    }
}