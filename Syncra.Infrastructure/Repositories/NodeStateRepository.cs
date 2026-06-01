using Microsoft.EntityFrameworkCore;
using Syncra.Application.Interfaces;

namespace Syncra.Infrastructure.Repositories
{
    public class NodeStateRepository : INodeStateRepository
    {
        private readonly SyncraDbContext _context;
        public NodeStateRepository(SyncraDbContext context)
        {
            _context = context;
        }

        public async Task<NodeState?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.NodeStates.AsNoTracking()
                .FirstOrDefaultAsync(n => n.node_id == id, cancellationToken);
        }

        public async Task<IEnumerable<NodeState>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.NodeStates.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task AddAsync(NodeState nodeState, CancellationToken cancellationToken = default)
        {
            await _context.NodeStates.AddAsync(nodeState, cancellationToken);
        }

        public async Task UpdateAsync(NodeState nodeState, CancellationToken cancellationToken = default)
        {
            _context.NodeStates.Update(nodeState);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var nodeState = await _context.NodeStates.FindAsync(new object[] { id }, cancellationToken);
            if (nodeState != null)
            {
                _context.NodeStates.Remove(nodeState);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
