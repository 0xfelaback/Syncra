public class Event
{
    public string event_id { get; set; } = null!; //pk
    public string? parent_event_id { get; set; } // previous one before
    public string? compensates_event_id { get; set; } // if compensated event, what event did this reverse
    public string node_id { get; set; } = null!; //index, node that created event
    public string account_id { get; set; } = null!; //index
    public int node_sequence { get; set; }
    public long server_sequence { get; set; } //unique, index
    public DateTime node_timestamp { get; set; }
    public DateTime server_timestamp { get; set; } //index
    public EventType Type { get; set; }
    public EventPayloadData payload { get; set; } = null!;
    public EventStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public enum EventType
    {
        AccountDebited = 1, AccountCredited, CompensatingTransaction
    }
    public enum EventStatus
    {
        Pending = 1, Accepted, Compensated
    }
    public class EventPayloadData
    {
        public decimal amount { get; set; }
        public string reason { get; set; } = null!;
    }
}

