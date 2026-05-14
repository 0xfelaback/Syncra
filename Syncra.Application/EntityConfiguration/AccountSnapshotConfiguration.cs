using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class AccountSnapshotConfiguration : IEntityTypeConfiguration<AccountSnapshot>
{
    public void Configure(EntityTypeBuilder<AccountSnapshot> builder)
    {
        builder.HasKey(a => a.snapshot_id);
        builder.Property(a => a.account_id).IsRequired();
        builder.HasIndex(a => a.account_id);
        builder.Property(a => a.snapshot_sequence).IsRequired();
        builder.Property(a => a.balance).IsRequired().HasPrecision(18, 2);
        builder.Property(a => a.version).IsRequired();
        builder.Property(a => a.event_count).IsRequired();
    }
}