using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Syncra.Domain.Entities
{
    public class IdempotencyKey
    {
        // 48hrs - cron
        public string idempotency_key { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("caused_event")]
        public string event_id { get; set; } = null!;
        public Event caused_event { get; set; } = null!;
        public int response_status { get; set; }
        public JsonDocument response_body { get; set; } = null!;
        public DateTimeOffset createdAt { get; set; } = DateTimeOffset.UtcNow;
        [Timestamp]
        public uint Version { get; set; }
    }
}