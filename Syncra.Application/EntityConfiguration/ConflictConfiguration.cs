using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class ConflictConfiguration : IEntityTypeConfiguration<Conflict>
{
    public void Configure(EntityTypeBuilder<Conflict> builder)
    {
        builder.HasKey(c => c.conflict_id);
        builder.HasIndex(c => c.account_id);
        builder.Property(c => c.account_id).IsRequired();
        builder.Property(c => c.original_event_id).IsRequired(false);
        builder.Property(c => c.compensation_event_id).IsRequired(false);
        builder.Property(c => c.original_event_archive_id).IsRequired(false);
        builder.Property(c => c.compensation_event_archive_id).IsRequired(false);
        builder.HasOne(c => c.original_event)
            .WithOne()
            .HasForeignKey<Conflict>(c => c.original_event_id).IsRequired(false);
        builder.HasOne(c => c.compensation_event)
            .WithOne()
            .HasForeignKey<Conflict>(c => c.compensation_event_id).IsRequired(false);
        builder.HasOne(c => c.original_event_archive)
            .WithOne()
            .HasForeignKey<Conflict>(c => c.original_event_archive_id).IsRequired(false);
        builder.HasOne(c => c.compensation_event_archive)
            .WithOne()
            .HasForeignKey<Conflict>(c => c.compensation_event_archive_id).IsRequired(false);
        builder.Property(c => c.Type).IsRequired();
        builder.Property(c => c.atempted_balance).IsRequired().HasPrecision(18, 2);
        builder.Property(c => c.actual_balance).IsRequired().HasPrecision(18, 2);
        builder.Property(c => c.resolution).IsRequired();
    }
}