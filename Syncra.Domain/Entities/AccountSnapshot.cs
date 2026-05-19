public class AccountSnapshot
{
    //Periodic checkpoints of account state for fast replay
    public int snapshot_id { get; set; }
    public string account_id { get; set; } = null!;
    public Account account { get; set; } = null!;
    public long snapshot_sequence { get; set; } // Server sequence at which snapshot was taken
    public decimal balance { get; set; }
    public int version { get; set; } // Account version at snapshot point
    public int event_count { get; set; } // Total events processed up to this point
    public DateTime created_at { get; set; } = DateTime.Now;
}