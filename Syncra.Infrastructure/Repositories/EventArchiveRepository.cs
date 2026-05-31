using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;
using Syncra.Domain.Entities;
using Syncra.Infrastructure;

namespace Syncra.Infrastructure.Repositories
{
    public class EventArchiveRepository : IEventArchiveRepository
    {
        private readonly SyncraDbContext _context;
        public EventArchiveRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<EventArchive?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.EventArchives
                .FirstOrDefaultAsync(e => e.event_id == id, cancellationToken);
        }

        public async Task<IEnumerable<EventArchive>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.EventArchives
                
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventArchive>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default)
        {
            return await _context.EventArchives
                
                .Where(e => e.aggregateId == accountId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(EventArchive eventArchive, CancellationToken cancellationToken = default)
        {
            await _context.EventArchives.AddAsync(eventArchive, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<EventArchive> eventArchives, CancellationToken cancellationToken = default)
        {
            await _context.EventArchives.AddRangeAsync(eventArchives, cancellationToken);
        }

        public async Task UpdateAsync(EventArchive eventArchive, CancellationToken cancellationToken = default)
        {
            _context.EventArchives.Update(eventArchive);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var eventArchive = await _context.EventArchives.FindAsync(new object[] { id }, cancellationToken);
            if (eventArchive != null)
            {
                _context.EventArchives.Remove(eventArchive);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
