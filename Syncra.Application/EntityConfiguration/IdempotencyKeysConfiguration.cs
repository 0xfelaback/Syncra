using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncra.Domain.Entities;

namespace Syncra.Application.EntityConfiguration
{
    public class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
    {
        public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
        {
            builder.HasKey(x => x.event_id);
            builder.HasOne(x => x.caused_event).WithOne(e => e.idempotency_record).HasForeignKey<IdempotencyKey>(x => x.event_id);
            builder.Property(x => x.response_body).IsRequired();
            builder.Property(x => x.response_status).IsRequired();
        }
    }
}