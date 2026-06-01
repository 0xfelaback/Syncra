

namespace Syncra.Application.Interfaces
{
    public interface IAccountStateRepository
    {
        Task<AccountState?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<AccountState>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(AccountState accountState, CancellationToken cancellationToken = default);
        Task UpdateAsync(AccountState accountState, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
