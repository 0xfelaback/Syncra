using System.ComponentModel.DataAnnotations.Schema;

public class Conflict
{
    // Audit trail of detected conflicts and resolutions - complete
    public int conflict_id { get; set; }
    public DateTime detected_at { get; set; } = DateTime.Now;
    public string account_id { get; set; } = null!;
    public Account account { get; set; } = null!;
    //[ForeignKey("original_event")]
    public string? original_event_id { get; set; } = null; // related to event record, event that caused conflict
    [InverseProperty(nameof(Event.caused_conflict))]
    public Event original_event { get; set; } = null!;
    //[ForeignKey("compensation_event")]
    public string? compensation_event_id { get; set; } = null; // compensation event generated e.g. server_compensation_001
    [InverseProperty(nameof(Event.compensates_conflict))]
    public Event compensation_event { get; set; } = null!;

    // Events Archive
    //[ForeignKey("original_event_archive")]
    public string? original_event_archive_id { get; set; } = null;
    [InverseProperty(nameof(EventArchive.caused_conflict))]
    public EventArchive original_event_archive { get; set; } = null!;
    //[ForeignKey("compensation_event_archive")]
    public string? compensation_event_archive_id { get; set; } = null;
    [InverseProperty(nameof(EventArchive.compensates_conflict))]
    public EventArchive compensation_event_archive { get; set; } = null!;

    public ConflictType Type { get; set; }
    public decimal atempted_balance { get; set; }
    public decimal actual_balance { get; set; }
    public Resolution resolution { get; set; }
    //public jsonb? metadata { get; set; } - I need a shape for this.

    public enum ConflictType
    {
        InsufficientFunds = 1, InvalidOperation
    }
    public enum Resolution
    {
        compensate, reject
    }
}