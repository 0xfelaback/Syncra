namespace Syncra.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task AddAsync(Account account, CancellationToken cancellationToken = default);
        Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
