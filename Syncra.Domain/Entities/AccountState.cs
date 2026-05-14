public class AccountState
{
    public string account_id { get; set; } = null!;
    public decimal balance { get; set; }
    public decimal provisional_balance { get; set; }
    public int version { get; set; }
    public string last_event_id { get; set; } = null!;
    public long last_server_sequence { get; set; }
    public DateTime UpdatedAt { get; set; }
}