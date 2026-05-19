using System.ComponentModel.DataAnnotations.Schema;

public class EventArchive
{
    // Store historical events older than 90 days
    public string event_id { get; set; } = null!;
    [ForeignKey("parent_event")]
    public string? parent_event_id { get; set; } = null;
    public EventArchive? parent_event { get; set; } = null;
    [ForeignKey("compensates_event")]
    public string? compensates_event_id { get; set; }
    public EventArchive? compensates_event { get; set; } = null;
    [ForeignKey("node")]
    public string node_id { get; set; } = null!;
    public NodeState node { get; set; } = null!;
    public int node_sequence { get; set; }
    public long server_sequence { get; set; }
    public DateTime node_timestamp { get; set; }
    public DateTime server_timestamp { get; set; }
    public EventArchiveType Type { get; set; }
    public EventArchivePayloadData payload { get; set; } = null!;
    public EventArchiveStatus Status { get; set; }
    public DateTime created_at { get; set; } = DateTime.Now;
    //[ForeignKey("caused_conflict")]
    public int? caused_conflict_id { get; set; } = null; // if event caused a conflict
    [InverseProperty(nameof(Conflict.original_event_archive))]
    public Conflict? caused_conflict { get; set; } = null;
    //[ForeignKey("compensates_conflict")]
    public int? compensates_conflict_id { get; set; } = null;
    [InverseProperty(nameof(Conflict.compensation_event_archive))]
    public Conflict? compensates_conflict { get; set; } = null; // if compensated event, what conflict did this settle   


    public enum EventArchiveType
    {
        AccountDebited = 1, AccountCredited, CompensatingTransaction, RejectedTransaction
    }
    public enum EventArchiveStatus
    {
        Pending = 1, Accepted, Compensated, Duplicate
    }

    public class EventArchivePayloadData
    {
        public decimal amount { get; set; }
        public string reason { get; set; } = null!;
        [ForeignKey("from_account")]
        public string from_account_id { get; set; } = null!;
        public Account from_account { get; set; } = null!;
        [ForeignKey("to_account")]
        public string to_account_id { get; set; } = null!;
        public Account to_account { get; set; } = null!;
    }

}