using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class AccountStateRepository : IAccountStateRepository
    {
        private readonly SyncraDbContext _context;
        public AccountStateRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<AccountState?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.AccountStates.AsNoTracking()
                .FirstOrDefaultAsync(s => s.account_id == id, cancellationToken);
        }

        public async Task<IEnumerable<AccountState>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AccountStates.AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(AccountState accountState, CancellationToken cancellationToken = default)
        {
            await _context.AccountStates.AddAsync(accountState, cancellationToken);
        }

        public async Task UpdateAsync(AccountState accountState, CancellationToken cancellationToken = default)
        {
            _context.AccountStates.Update(accountState);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var accountState = await _context.AccountStates.FindAsync(new object[] { id }, cancellationToken);
            if (accountState != null)
            {
                _context.AccountStates.Remove(accountState);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
