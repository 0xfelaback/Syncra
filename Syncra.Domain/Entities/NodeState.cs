public class NodeState
{
    //Track last known state of each node (for debugging/monitoring)
    public string node_id { get; set; } = null!;
    public string status { get; set; } = null!;
    public int local_sequence { get; set; }
    public long last_synced_server_sequence { get; set; }
    public int pending_events_count { get; set; } = 0;
    public DateTime? last_sync_attempt { get; set; } = null;
    public DateTime? last_successful_sync { get; set; } = null;
    public DateTime last_seen { get; set; } = DateTime.Now;
    public virtual ICollection<Event> events { get; set; } = new List<Event>();
    public virtual ICollection<EventArchive> eventsArchive { get; set; } = new List<EventArchive>();
}