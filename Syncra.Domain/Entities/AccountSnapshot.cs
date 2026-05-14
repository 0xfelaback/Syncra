public class AccountSnapshot
{
    public int snapshot_id { get; set; }
    public string account_id { get; set; } = null!;
    public long snapshot_sequence { get; set; }
    public decimal balance { get; set; }
    public int version { get; set; }
    public int event_count { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}