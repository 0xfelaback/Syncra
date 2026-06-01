using Microsoft.EntityFrameworkCore;
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
    }
}