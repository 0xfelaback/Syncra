using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class ConflictRepository : IConflictRepository
    {
        private readonly SyncraDbContext _context;
        public ConflictRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<Conflict?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Conflicts.AsNoTracking().FirstOrDefaultAsync(c => c.conflict_id == id, cancellationToken);
        }

        public async Task<IEnumerable<Conflict>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Conflicts.AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Conflict>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Conflicts
                .Where(c => c.account_id == accountId).AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Conflict conflict, CancellationToken cancellationToken = default)
        {
            await _context.Conflicts.AddAsync(conflict, cancellationToken);
        }

        public async Task UpdateAsync(Conflict conflict, CancellationToken cancellationToken = default)
        {
            _context.Conflicts.Update(conflict);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var conflict = await _context.Conflicts.FindAsync(new object[] { id }, cancellationToken);
            if (conflict != null)
            {
                _context.Conflicts.Remove(conflict);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
