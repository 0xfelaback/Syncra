using Syncra.Domain.Entities;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetByAccountIdAsync(string accountId, CancellationToken cancellationToken = default);
    Task<long?> GetLastServerSequence(CancellationToken cancellationToken = default);
    Task AddAsync(Event e, CancellationToken cancellationToken = default);
    Task AddCollectionOfEvents(ICollection<Event> events, CancellationToken cancellationToken = default);
    Task UpdateAsync(Event e, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
