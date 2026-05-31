using Syncra.Domain.Entities;

namespace Syncra.Application.Interfaces
{
    public interface IIdempotencyKeysRepository
    {
        Task<IdempotencyKey?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<IdempotencyKey>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> CheckThatEventIdExists(string eventId, CancellationToken cancellationToken = default);
        Task AddAsync(IdempotencyKey idempotencyKey, CancellationToken cancellationToken = default);
        Task UpdateAsync(IdempotencyKey idempotencyKey, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}