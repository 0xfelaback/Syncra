using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly SyncraDbContext _context;
        public EventRepository(SyncraDbContext context)
        {
            _context = context;
        }
        public async Task<Event?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Events.AsNoTracking()
                .FirstOrDefaultAsync(e => e.event_id == id, cancellationToken);
        }
        public async Task<bool> VerifyEventIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Events.AnyAsync(e => e.event_id == id, cancellationToken);
        }

        public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Events.AsNoTracking().ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Event>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Events.AsNoTracking().Where(e => e.aggregateId == accountId)
                .ToListAsync(cancellationToken);
        }
        public async Task<long?> GetLastServerSequence(CancellationToken cancellationToken = default)
        {
            return await _context.Events
                .Where(e => e.server_sequence != null).AsNoTracking()
                .OrderByDescending(e => e.event_id)
                .Select(e => e.server_sequence)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<Event>?> GetEventsSinceLastSnap(long lastServerSequence, long newServerSequence, string accountId)
        {
            return await _context.Events.Where(e => e.server_sequence > lastServerSequence && e.server_sequence < newServerSequence && e.aggregateId == accountId).OrderBy(e => e.server_sequence).AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Event e, CancellationToken cancellationToken = default) => await _context.Events.AddAsync(e, cancellationToken);
        public async Task AddCollectionOfEvents(ICollection<Event> events, CancellationToken cancellationToken = default)
        {
            await _context.Events.AddRangeAsync(events, cancellationToken);
        }

        public async Task UpdateAsync(Event e, CancellationToken cancellationToken = default) => _context.Events.Update(e);

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var e = await _context.Events.FindAsync([id], cancellationToken);
            if (e != null)
            {
                _context.Events.Remove(e);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<long> GetNextServerSequenceAsync(CancellationToken cancellationToken = default)
        {
            var projectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Syncra.Api"));
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
            const string sql = "SELECT nextval('event_server_sequence_seq');";

            await using var connection = new NpgsqlConnection(configuration.GetConnectionString("localConnectionString"));
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("Failed to generate next sequence value.");
            }
            return Convert.ToInt64(result);
        }
    }
}