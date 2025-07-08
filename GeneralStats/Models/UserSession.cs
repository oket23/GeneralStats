namespace GeneralStats.Models;

public class UserSession
{
    public string UserStatus { get; set; } = "null";
    public int Page { get; set; } = 1;

    public string LastPlayerTag { get; set; }
    public int? LastMessageId { get; set; }
}
