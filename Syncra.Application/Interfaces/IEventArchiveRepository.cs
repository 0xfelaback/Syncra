

namespace Syncra.Application.Interfaces
{
    public interface IEventArchiveRepository
    {
        Task<EventArchive?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventArchive>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<EventArchive>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default);
        Task AddAsync(EventArchive eventArchive, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<EventArchive> eventArchives, CancellationToken cancellationToken = default);
        Task UpdateAsync(EventArchive eventArchive, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
