using Syncra.Domain.Entities;

namespace Syncra.Application.Interfaces
{
    public interface IConflictRepository
    {
        Task<Conflict?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Conflict>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Conflict>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default);
        Task AddAsync(Conflict conflict, CancellationToken cancellationToken = default);
        Task UpdateAsync(Conflict conflict, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
