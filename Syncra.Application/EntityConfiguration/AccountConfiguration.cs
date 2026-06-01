using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccountCofiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.account_id);
        builder.HasIndex(a => a.userId);
        builder.Property(a => a.userId).IsRequired();
        builder.HasOne(a => a.account_state).WithOne(a => a.account);
        builder.HasMany(a => a.account_snapshots).WithOne(a => a.account).HasForeignKey(a => a.account_id);
        builder.HasMany(a => a.conflicts).WithOne(a => a.account).HasForeignKey(a => a.account_id);
        builder.Property(a => a.Version).HasColumnName("version_control");
        builder.HasMany(a => a.events).WithOne(e => e.aggregate).HasForeignKey(e => e.aggregateId);
        builder.HasMany(a => a.event_archives).WithOne(e => e.aggregate).HasForeignKey(e => e.aggregateId);
    }
}