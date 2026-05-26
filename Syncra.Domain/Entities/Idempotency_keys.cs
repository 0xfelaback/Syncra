using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Syncra.Domain.Entities
{
    public class IdempotencyKey
    {
        // 48hrs
        [ForeignKey("caused_event")]
        public string event_id { get; set; } = null!;
        public Event caused_event { get; set; } = null!;
        public int response_status { get; set; } // not null
        public JsonDocument response_body { get; set; } = null!; // not null
        public DateTimeOffset createdAt { get; set; } = DateTimeOffset.UtcNow;
    }
}