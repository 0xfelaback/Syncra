using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class IdempotencyKeysRepository : IIdempotencyKeysRepository
    {
        private readonly SyncraDbContext _context;
        public IdempotencyKeysRepository(SyncraDbContext context)
        {
            _context = context;
        }
        public async Task<bool> checkThatEventIdExists(string eventId) => await _context.IdempotencyKeys.AnyAsync(x => x.event_id == eventId);
    }
}