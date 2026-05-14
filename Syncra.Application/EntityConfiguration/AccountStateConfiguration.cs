using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class AccountStateConfiguration : IEntityTypeConfiguration<AccountState>
{
    public void Configure(EntityTypeBuilder<AccountState> builder)
    {
        builder.HasKey(a => a.account_id);
        builder.Property(a => a.balance).IsRequired().HasPrecision(18, 2);
        builder.Property(a => a.provisional_balance).IsRequired().HasPrecision(18, 2);
        builder.Property(a => a.version).IsRequired();
        builder.Property(a => a.last_event_id).IsRequired();
        builder.HasOne(a => a.last_event_id).WithOne().HasForeignKey<Event>(e => e.event_id);
        builder.Property(a => a.last_server_sequence).IsRequired();
    }
}