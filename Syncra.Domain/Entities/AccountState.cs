using System.ComponentModel.DataAnnotations.Schema;

public class AccountState
{
    // Materialized view of current account balances (derived from events)
    [ForeignKey("account")]
    public string account_id { get; set; } = null!;
    public Account account { get; set; } = null!;
    public decimal balance { get; set; }
    public decimal provisional_balance { get; set; } // Balance including pending events
    public int version { get; set; }
    [ForeignKey("last_event")]
    public string? last_event_id { get; set; } = null; // last event synced by account
    public Event last_event { get; set; } = null!;
    public long? last_server_sequence { get; set; } = null;
    public DateTime updated_at { get; set; } = DateTime.Now;
}