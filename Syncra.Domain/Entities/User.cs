public class User
{
    public int userId { get; set; }
    public string userName { get; set; } = null!;
    public string passwordhash { get; set; } = null!;
    public virtual ICollection<Account> accounts { get; set; } = new List<Account>();
}