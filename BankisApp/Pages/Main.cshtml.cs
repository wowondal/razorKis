using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

public class MainModel : PageModel
{
    [BindProperty]
    public string? Username { get; set; }

    [BindProperty]
    public string? AccountNo { get; set; }

    [BindProperty]
    public string? AppKey { get; set; }

    [BindProperty]
    public string? AppSecret { get; set; }

    public List<AccountInfo> UserAccounts { get; set; } = new List<AccountInfo>();

    public void OnGet()
    {
        // 초기화 코드
        Username = Request.Cookies["LoginCookie"]?.ToString();

        // 사용자 계좌 정보를 로드합니다.
        LoadUserAccounts();
    }

    public async Task<IActionResult> OnPostEnrollAccountAsync()
    {
        var accountInfo = new AccountInfo
        {
            AccountNo = AccountNo,
            AppKey = AppKey,
            AppSecret = AppSecret
        };

        var userFilePath = "./user.txt";
        var userAccounts = new Dictionary<string, UserData>();

        if (System.IO.File.Exists(userFilePath))
        {
            var lines = await System.IO.File.ReadAllLinesAsync(userFilePath);
            foreach (var line in lines)
            {
                var userData = JsonSerializer.Deserialize<UserData>(line);
                if (userData != null && !userAccounts.ContainsKey(userData.Username))
                {
                    userAccounts[userData.Username] = userData;
                }
            }
        }

        if (Username != null && !userAccounts.ContainsKey(Username))
        {
            userAccounts[Username] = new UserData
            {
                Username = Username,
                Accounts = new List<AccountInfo>()
            };
        }

        if (Username != null)
        {
            if (userAccounts[Username].Accounts == null)
            {
                userAccounts[Username].Accounts = new List<AccountInfo>();
            }
            userAccounts[Username].Accounts.Add(accountInfo);
        }

        var updatedLines = new List<string>();
        foreach (var user in userAccounts.Values)
        {
            updatedLines.Add(JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true }));
        }
        await System.IO.File.WriteAllLinesAsync(userFilePath, updatedLines);

        // 사용자 계좌 정보를 다시 로드합니다.
        LoadUserAccounts();

        return RedirectToPage();
    }

    private void LoadUserAccounts()
    {
        var userFilePath = "./user.txt";
        var userAccounts = new Dictionary<string, UserData>();

        if (System.IO.File.Exists(userFilePath))
        {
            var lines = System.IO.File.ReadAllLines(userFilePath);
            foreach (var line in lines)
            {
                var userData = JsonSerializer.Deserialize<UserData>(line);
                if (userData != null && !userAccounts.ContainsKey(userData.Username))
                {
                    userAccounts[userData.Username] = userData;
                }
            }
        }

        if (Username != null && userAccounts.ContainsKey(Username))
        {
            UserAccounts = userAccounts[Username].Accounts ?? new List<AccountInfo>();
        }
        else
        {
            UserAccounts = new List<AccountInfo>();
        }
    }
}