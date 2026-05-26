public class Account
{
    // A user account - the important data for account is held at account state
    public string account_id { get; set; } = null!;
    public int userId { get; set; }
    public User user { get; set; } = null!;
    public AccountState account_state { get; set; } = null!;
    public virtual ICollection<AccountSnapshot> account_snapshots { get; set; } = new List<AccountSnapshot>();
    public virtual ICollection<Conflict> conflicts { get; set; } = [];
    public virtual ICollection<Event> events { get; set; } = new List<Event>();
    public virtual ICollection<EventArchive> event_archives { get; set; } = new List<EventArchive>();
}