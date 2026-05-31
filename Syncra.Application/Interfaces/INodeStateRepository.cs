namespace Syncra.Application.Interfaces
{
    public interface INodeStateRepository
    {
        Task<NodeState?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<NodeState>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(NodeState nodeState, CancellationToken cancellationToken = default);
        Task UpdateAsync(NodeState nodeState, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
