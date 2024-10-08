using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Web;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string RegisterUsername { get; set; }

    [BindProperty]
    public string RegisterPassword { get; set; }

    [BindProperty]
    public string RegisterEmail { get; set; }

    public string LoginError { get; set; }
    public string RegisterError { get; set; }

    public void OnGet()
    {
        // TempData에서 오류 메시지를 가져옴
        if (TempData.ContainsKey("LoginError"))
        {
            LoginError = TempData["LoginError"].ToString();
        }

        if (TempData.ContainsKey("RegisterError"))
        {
            RegisterError = TempData["RegisterError"].ToString();
        }
    }

    public IActionResult OnPost()
    {
        string filePath = "./user.txt";

        if (System.IO.File.Exists(filePath))
        {
            var users = System.IO.File.ReadAllLines(filePath)
                .Select(line => JsonSerializer.Deserialize<UserInfo>(line));

            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
            if (user != null)
            {
                // 로그인 성공 시 쿠키 설정
                Response.Cookies.Append("LoginCookie", Username);

                // main 페이지로 리다이렉트
                return RedirectToPage("/Main");
            }
        }

        // 로그인 실패 시 TempData에 오류 메시지 저장
        TempData["LoginError"] = "로그인이 실패하였습니다.\\n아이디, 비밀번호를 다시 확인해 주세요.";

        // 현재 페이지로 리디렉트
        return RedirectToPage();
    }

    public IActionResult OnPostRegister()
    {
        if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
        {
            TempData["RegisterError"] = "Username과 Password는 필수입니다.";
            return RedirectToPage();
        }

        string filePath = "./user.txt";
        List<UserInfo> users = new List<UserInfo>();

        // 사용자 이름이 이미 존재하는지 확인
        if (System.IO.File.Exists(filePath))
        {
            var existingUsers = System.IO.File.ReadAllLines(filePath)
                .Select(line =>
                {
                    try
                    {
                        return JsonSerializer.Deserialize<UserInfo>(line);
                    }
                    catch (JsonException)
                    {
                        // JSON 파싱 오류가 발생하면 null 반환
                        return null;
                    }
                })
                .Where(user => user != null); // null이 아닌 사용자만 필터링

            if (existingUsers.Any(user => user.Username == RegisterUsername))
            {
                TempData["RegisterError"] = "이미 존재하는 사용자 이름입니다.";
                return RedirectToPage();
            }

            users.AddRange(existingUsers);
        }

        // 사용자 정보를 객체로 생성
        var newUser = new UserInfo
        {
            Username = RegisterUsername,
            Password = RegisterPassword,
            Email = RegisterEmail
        };

        users.Add(newUser);

        try
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (var user in users)
                {
                    string json = JsonSerializer.Serialize(user);
                    sw.WriteLine(json);
                }
            }
        }
        catch (Exception ex)
        {
            // 파일 쓰기 오류 처리
            TempData["RegisterError"] = "사용자 정보를 저장하는 중 오류가 발생했습니다.";
            return RedirectToPage();
        }

        // 임시로 등록 후 인덱스 페이지로 리디렉션
        TempData["RegisterError"] = "성공적으로 등록되었습니다.";
        return RedirectToPage();
    }
}
