using Syncra.Domain.Entities;

namespace Syncra.Application.Interfaces
{
    public interface IAccountSnapshotRepository
    {
        Task<AccountSnapshot?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<AccountSnapshot>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<AccountSnapshot>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default);
        Task AddAsync(AccountSnapshot snapshot, CancellationToken cancellationToken = default);
        Task UpdateAsync(AccountSnapshot snapshot, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
