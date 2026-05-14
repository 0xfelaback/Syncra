public class NodeState
{
    public string node_id { get; set; } = null!;
    public string status { get; set; } = null!;
    public int local_sequence { get; set; }
    public long last_synced_server_sequence { get; set; }
    public int pending_events_count { get; set; } = 0;
    public DateTime? last_sync_attempt { get; set; }
    public DateTime? last_successful_sync { get; set; }
    public DateTime last_seen { get; set; } = DateTime.Now;
}