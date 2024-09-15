using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IO;
using eFriendOpenAPI;
using eFriendOpenAPI.Packet;
using eFriendOpenAPI.Extension;
using System.Security.Principal;
using System.Text;


public class MainModel : PageModel
{
    #region properties
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string AccountNo { get; set; }

    [BindProperty]
    public string AppKey { get; set; }

    [BindProperty]
    public string AppSecret { get; set; }

    public List<AccountInfo> UserAccounts { get; set; } = new List<AccountInfo>();

    public string Result { get; set; }

    public string MyStocks { get; set; }

    public Dictionary<string, KOSPICode> KospiList { get; set; }

    public Dictionary<string, KOSDAQCode> KosdaqList { get; set; }
    #endregion

    private string GetAccountFilePath()
    {
        string name = Request.Cookies["LoginCookie"] + "";
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "UserData", name);

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string filePath = Path.Combine(dir, "account.txt");

        // 파일이 존재하지 않으면 생성
        if (!System.IO.File.Exists(filePath))
        {
            using (var fileStream = System.IO.File.Create(filePath))
            {
                // 파일 스트림을 닫기 위해 using 블록 사용
            }
        }

        return filePath;
    }

    private bool IsAleadyEnrolled(string accountNo)
    {
        return UserAccounts.Any(a => a.AccountNo == accountNo);
    }

    private void LoadUserAccounts()
    {
        UserAccounts.Clear();

        var accountFilePath = GetAccountFilePath();
        if (System.IO.File.Exists(accountFilePath))
        {
            foreach (var line in System.IO.File.ReadAllLines(accountFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var account = JsonSerializer.Deserialize<AccountInfo>(line);
                    if (account != null && account.Username == Username)
                    {
                        UserAccounts.Add(account);
                    }
                }
                catch (JsonException je)
                {
                    TempData["Result"] = $"JSON 파싱 중 오류 발생({je.Message})";
                    break;
                }
            }
        }
    }

    private void Init()
    {
        // 초기화 코드
        Username = Request.Cookies["LoginCookie"]?.ToString();
        if (Username == null)
        {
            Response.Redirect("/Account/Login");
        }

        // 사용자 계좌 정보를 로드합니다.
        LoadUserAccounts();
    }

    public void OnGet()
    {
        Init();

        // TempData에서 결과 메시지를 가져옵니다.
        if (TempData.ContainsKey("Result"))
        {
            Result = TempData["Result"].ToString();
        }
    }

    public async Task<IActionResult> OnPostEnrollAccountAsync()
    {
        Username = Request.Cookies["LoginCookie"]?.ToString();
        if (Username == null)
        {
            Response.Redirect("/Account/Login");
        }

        // 사용자 계좌 정보를 로드합니다.
        LoadUserAccounts();

        // 이미 등록된 계좌인지 확인합니다.
        if (IsAleadyEnrolled(AccountNo))
        {
            TempData["Result"] = $"이미 존재하는 계좌입니다. ({AccountNo})";
            return RedirectToPage();
        }

        // 계좌 정보를 저장합니다.
        var accountInfo = new AccountInfo
        {
            Username = Username,
            AccountNo = AccountNo,
            AppKey = AppKey,
            AppSecret = AppSecret
        };

        var accountFilePath = GetAccountFilePath();

        // 계좌 정보를 JSON 형태로 저장합니다.
        var accountJson = JsonSerializer.Serialize(accountInfo, new JsonSerializerOptions { WriteIndented = false });
        await System.IO.File.AppendAllTextAsync(accountFilePath, accountJson + Environment.NewLine);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAccountAsync()
    {
        Username = Request.Cookies["LoginCookie"]?.ToString();
        if (Username == null)
        {
            Response.Redirect("/Account/Login");
        }

        // 사용자 계좌 정보를 로드합니다.
        LoadUserAccounts();

        // 계좌 정보를 수정합니다.
        var account = UserAccounts.FirstOrDefault(a => a.Username == Username && a.AccountNo == AccountNo);
        if (account != null)
        {
            account.AppKey = AppKey;
            account.AppSecret = AppSecret;

            var accountFilePath = GetAccountFilePath();
            var updatedAccounts = UserAccounts.Select(a => JsonSerializer.Serialize(a, new JsonSerializerOptions { WriteIndented = false }));
            await System.IO.File.WriteAllLinesAsync(accountFilePath, updatedAccounts);

            TempData["Result"] = "계좌가 성공적으로 수정되었습니다.";
        }
        else
        {
            TempData["Result"] = "계좌를 찾을 수 없습니다.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAccountAsync()
    {
        Username = Request.Cookies["LoginCookie"]?.ToString();
        if (Username == null)
        {
            Response.Redirect("/Account/Login");
        }

        // 사용자 계좌 정보를 로드합니다.
        LoadUserAccounts();

        // 계좌 정보를 삭제합니다.
        var account = UserAccounts.FirstOrDefault(a => a.Username == Username && a.AccountNo == AccountNo);
        if (account != null)
        {
            UserAccounts.Remove(account);

            var accountFilePath = GetAccountFilePath();
            var updatedAccounts = UserAccounts.Select(a => JsonSerializer.Serialize(a, new JsonSerializerOptions { WriteIndented = false }));
            await System.IO.File.WriteAllLinesAsync(accountFilePath, updatedAccounts);

            TempData["Result"] = "계좌가 성공적으로 삭제되었습니다.";
        }
        else
        {
            TempData["Result"] = "계좌를 찾을 수 없습니다.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostGenerateToken(string accountNo, string appKey, string appSecret)
    {
        bool isVTS = false; // true: 모의 Domain, false: 실전 Domain
        eFriendClient client = new eFriendClient(isVTS, appKey, appSecret, accountNo);

        string kisDirectory = Path.Combine(Directory.GetCurrentDirectory(), "eFriendOpenAPI");
        await client.LoadKospiMasterCode(kisDirectory);
        await client.LoadKosdaqiMasterCode(kisDirectory);

        if (await client.CheckAccessToken() == false)
        {
            TempData["Result"] = "Failed to get AccessToken";
            return RedirectToPage();
        }

        // 결과 처리
        TempData["Result"] = "토큰이 성공적으로 생성되었습니다.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostGetStockList(string accountNo, string appKey, string appSecret)
    {
        bool isVTS = false; // true: 모의 Domain, false: 실전 Domain
        eFriendClient client = new eFriendClient(isVTS, appKey, appSecret, accountNo);

        string kisDirectory = Path.Combine(Directory.GetCurrentDirectory(), "eFriendOpenAPI");
        KospiList = await client.LoadKospiMasterCode(kisDirectory);
        KosdaqList = await client.LoadKosdaqiMasterCode(kisDirectory);

        Init();

        return Page();
    }

    public async Task<IActionResult> OnPostShowMyStocks(string accountNo, string appKey, string appSecret)
    {
        if (string.IsNullOrWhiteSpace(accountNo) || string.IsNullOrWhiteSpace(appKey) || string.IsNullOrWhiteSpace(appSecret))
        {
            TempData["Result"] = "계좌 정보를 입력해주세요.";
            return RedirectToPage();
        }

        bool isVTS = false; // true: 모의 Domain, false: 실전 Domain
        eFriendClient client = new eFriendClient(isVTS, appKey, appSecret, accountNo);

        string kisDirectory = Path.Combine(Directory.GetCurrentDirectory(), "eFriendOpenAPI");
        await client.LoadKospiMasterCode(kisDirectory);
        await client.LoadKosdaqiMasterCode(kisDirectory);

        if (await client.CheckAccessToken() == false)
        {
            TempData["Result"] = "AccessToken 이 없습니다.";
            return RedirectToPage();
        }

        // 보유 주식 조회
        var array = await client.주식잔고조회();
        if (array == null)
        {
            TempData["Result"] = "보유 주식이 없습니다.";
            return RedirectToPage();
        }

        StringBuilder sb = new StringBuilder();
        int i = 0;
        foreach (주식잔고조회DTO? dto in array)
        {
            if (dto?.evlu_pfls_amt.ToMoney() > 0)
            {
                sb.Append("<tr class='highlight'>");
            }
            else
            {
                sb.Append("<tr>");
            }
            sb.Append($"<td>{++i}</td>");
            sb.Append($"<td>{dto?.pdno}</td>");
            sb.Append($"<td>{dto?.prdt_name}</td>");
            sb.Append($"<td>{dto?.prpr.ToMoney().ToString("#,#")}</td>");
            sb.Append($"<td>{dto?.pchs_avg_pric.ToMoney().ToString("#,#")}</td>");
            sb.Append($"<td>{dto?.hldg_qty}</td>");
            sb.Append($"<td>{dto?.pchs_amt.ToMoney().ToString("#,#")}</td>");
            sb.Append($"<td>{dto?.evlu_pfls_amt.ToMoney().ToString("#,#")}</td>");
            sb.Append($"<td>{dto?.evlu_pfls_rt.ToMoney().ToString("#,#")}</td>");
            sb.Append($"</tr>");

            //sb.Append($"<li>{dto}");
            //var 시세 = await client.주식현재가시세(dto.pdno);
            //sb.Append($", 현재가={시세?.stck_prpr.ToMoney():n0}");
        }

        MyStocks = sb.ToString();

        Init();

        return Page();
    }
}