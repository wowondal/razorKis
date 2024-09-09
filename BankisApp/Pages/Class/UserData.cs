public class UserData
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? Email { get; set; }
    public string RegistrationDate { get; set; }
    public List<AccountInfo>? Accounts { get; set; } = new List<AccountInfo>();
}
