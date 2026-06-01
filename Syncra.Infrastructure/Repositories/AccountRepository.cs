using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SyncraDbContext _context;
        public AccountRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.Where(a => a.account_id == id).AsNoTracking()
                .FirstOrDefaultAsync(a => a.account_id == id, cancellationToken);
        }

        public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Accounts.AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.userId == userId).AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
        {
            await _context.Accounts.AddAsync(account, cancellationToken);
        }

        public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        {
            _context.Accounts.Update(account);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var account = await _context.Accounts.FindAsync(new object[] { id }, cancellationToken);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
