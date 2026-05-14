using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.event_id);
        builder.Property(e => e.node_id).IsRequired();
        builder.HasIndex(e => e.node_id).IsUnique();
        builder.Property(e => e.node_sequence).IsRequired();
        builder.Property(e => e.server_sequence).IsRequired();
        builder.HasIndex(e => e.server_sequence).IsUnique();
        builder.Property(e => e.node_timestamp).IsRequired();
        builder.Property(e => e.server_sequence).IsRequired();
        builder.HasIndex(e => e.server_timestamp);
        builder.Property(e => e.Type).IsRequired();
        builder.Property(e => e.account_id).IsRequired();
        builder.HasIndex(e => e.account_id);
        builder.Property(e => e.payload).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.HasOne(e => e.parent_event_id);
        builder.HasOne(e => e.compensates_event_id);
        builder.HasOne(e => e.parent_event_id).WithOne().HasForeignKey<Event>(e => e.parent_event_id);
        builder.HasOne(e => e.compensates_event_id).WithOne().HasForeignKey<Event>(e => e.compensates_event_id);
        builder.ToTable("Events", t => t.HasCheckConstraint(
        "CK_Event_CompensatingTransaction_NotNull",
        "([Type] <> 'CompensatingTransaction' OR [compensates_event_id] IS NOT NULL)"));

    }
}