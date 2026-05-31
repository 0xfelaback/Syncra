using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;
using Syncra.Domain.Entities;
using Syncra.Infrastructure;

namespace Syncra.Infrastructure.Repositories
{
    public class IdempotencyKeysRepository : IIdempotencyKeysRepository
    {
        private readonly SyncraDbContext _context;
        public IdempotencyKeysRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<IdempotencyKey?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.IdempotencyKeys
                .FirstOrDefaultAsync(i => i.idempotency_key == id, cancellationToken);
        }

        public async Task<IEnumerable<IdempotencyKey>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.IdempotencyKeys.ToListAsync(cancellationToken);
        }

        public async Task<bool> CheckThatEventIdExists(string eventId, CancellationToken cancellationToken = default)
        {
            return await _context.IdempotencyKeys.AnyAsync(x => x.event_id == eventId, cancellationToken);
        }

        public async Task AddAsync(IdempotencyKey idempotencyKey, CancellationToken cancellationToken = default)
        {
            await _context.IdempotencyKeys.AddAsync(idempotencyKey, cancellationToken);
        }

        public async Task UpdateAsync(IdempotencyKey idempotencyKey, CancellationToken cancellationToken = default)
        {
            _context.IdempotencyKeys.Update(idempotencyKey);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var idempotencyKey = await _context.IdempotencyKeys.FindAsync(new object[] { id }, cancellationToken);
            if (idempotencyKey != null)
            {
                _context.IdempotencyKeys.Remove(idempotencyKey);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}