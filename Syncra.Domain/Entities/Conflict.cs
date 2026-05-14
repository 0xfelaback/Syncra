public class Conflict
{
    public int conflict_id { get; set; }
    public DateTime detected_at { get; set; } = DateTime.Now;
    public string account_id { get; set; } = null!;
    public string original_event_id { get; set; } = null!;
    public string compensation_event_id { get; set; } = null!;
    public ConflictType Type { get; set; }
    public decimal attempted_balance { get; set; }
    public decimal actual_balance { get; set; }
    public Resolution resolution { get; set; }
    //public jsonb? metadata { get; set; } - I need a shape for this at least.

    public enum ConflictType
    {
        InsufficientFunds = 1, InvalidOperation
    }
    public enum Resolution
    {
        compensate, reject
    }
}