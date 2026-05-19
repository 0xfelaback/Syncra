using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.event_id);
        builder.Property(e => e.node_id).IsRequired();
        builder.HasIndex(e => e.node_id);
        builder.Property(e => e.node_sequence).IsRequired();
        builder.Property(e => e.server_sequence).IsRequired();
        builder.HasIndex(e => e.server_sequence).IsUnique();
        builder.Property(e => e.node_timestamp).IsRequired();
        builder.Property(e => e.server_sequence).IsRequired();
        builder.Property(e => e.created_at).IsRequired();
        builder.HasIndex(e => e.server_timestamp);
        builder.Property(e => e.Type).IsRequired();
        builder.OwnsOne(e => e.payload, payload =>
        {
            payload.Property(p => p.from_account_id).IsRequired();
            payload.HasOne(p => p.from_account).WithMany().HasForeignKey(p => p.from_account_id);
            payload.Property(p => p.to_account_id).IsRequired();
            payload.HasOne(p => p.to_account).WithMany().HasForeignKey(p => p.to_account_id);
        });
        builder.Property(e => e.Status).IsRequired();
        builder.HasOne(e => e.parent_event)
            .WithOne()
            .HasForeignKey<Event>(e => e.parent_event_id).IsRequired(false);
        builder.HasOne(e => e.compensates_event)
            .WithOne()
            .HasForeignKey<Event>(e => e.compensates_event_id).IsRequired(false);
        builder.HasOne(e => e.node).WithMany(n => n.events).HasForeignKey(e => e.node_id);
        builder.HasOne(e => e.caused_conflict)
            .WithOne()
            .HasForeignKey<Event>(e => e.caused_conflict_id).IsRequired(false);
        builder.HasOne(e => e.compensates_conflict)
            .WithOne()
            .HasForeignKey<Event>(e => e.compensates_conflict_id).IsRequired(false);



    }
}