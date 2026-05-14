public class EventArchive
{
    public string event_id { get; set; } = null!;
    public string? parent_event_id { get; set; }
    public string? compensates_event_id { get; set; }
    public string node_id { get; set; } = null!;
    public string account_id { get; set; } = null!;
    public int node_sequence { get; set; }
    public long server_sequence { get; set; }
    public DateTime node_timestamp { get; set; }
    public DateTime server_timestamp { get; set; }
    public Event.EventType Type { get; set; }
    public Event.EventPayloadData payload { get; set; } = null!;
    public Event.EventStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}