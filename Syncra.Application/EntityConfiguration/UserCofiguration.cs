using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserCofiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.userId);
        builder.Property(u => u.userName).IsRequired();
        builder.Property(u => u.passwordhash).IsRequired();
        builder.HasMany(u => u.accounts).WithOne(a => a.user).HasForeignKey(a => a.userId);
    }
}