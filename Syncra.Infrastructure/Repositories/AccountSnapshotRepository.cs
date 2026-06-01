using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class AccountSnapshotRepository : IAccountSnapshotRepository
    {
        private readonly SyncraDbContext _context;
        public AccountSnapshotRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<AccountSnapshot?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AccountSnapshots.AsNoTracking()
                .FirstOrDefaultAsync(s => s.snapshot_id == id, cancellationToken);
        }

        public async Task<IEnumerable<AccountSnapshot>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AccountSnapshots.AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AccountSnapshot>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default)
        {
            return await _context.AccountSnapshots.AsNoTracking()
                .Where(s => s.account_id == accountId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(AccountSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            await _context.AccountSnapshots.AddAsync(snapshot, cancellationToken);
        }

        public async Task UpdateAsync(AccountSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            _context.AccountSnapshots.Update(snapshot);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var snapshot = await _context.AccountSnapshots.FindAsync(new object[] { id }, cancellationToken);
            if (snapshot != null)
            {
                _context.AccountSnapshots.Remove(snapshot);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
