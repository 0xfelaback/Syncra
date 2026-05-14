using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class NodeStateConfiguration : IEntityTypeConfiguration<NodeState>
{
    public void Configure(EntityTypeBuilder<NodeState> builder)
    {
        builder.HasKey(n => n.node_id);
        builder.Property(n => n.status).IsRequired();
        builder.Property(n => n.local_sequence).IsRequired();
        builder.Property(n => n.last_synced_server_sequence).IsRequired();
    }
}