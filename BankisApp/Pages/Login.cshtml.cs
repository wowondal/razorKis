using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

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
        
    }

    public IActionResult OnPost()
    {
        ModelState.ClearValidationState(nameof(RegisterError));

        string filePath = "./user.txt";

        if (System.IO.File.Exists(filePath))
        {
            var users = System.IO.File.ReadAllLines(filePath)
                .Select(line => JsonSerializer.Deserialize<UserInfo>(line));

            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
            if (user != null)
            {
                // 로그인 성공 시 쿠키 설정
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Append("LoginCookie", Username, options);

                // main 페이지로 리다이렉트
                return RedirectToPage("/Main");
            }
        }

        // 로그인 실패 시
        ModelState.AddModelError("LoginError", "Invalid login attempt.");
        return Page();
    }

    public IActionResult OnPostRegister()
    {
        ModelState.ClearValidationState(nameof(LoginError));

        string filePath = "./user.txt";
        List<UserInfo> users = new List<UserInfo>();

        // 사용자 이름이 이미 존재하는지 확인
        if (System.IO.File.Exists(filePath))
        {
            var existingUsers = System.IO.File.ReadAllLines(filePath)
                .Select(line => JsonSerializer.Deserialize<UserInfo>(line));

            if (existingUsers.Any(user => user.Username == RegisterUsername))
            {
                ModelState.AddModelError("RegisterError", "이미 존재하는 사용자 이름입니다.");
                return Page();
            }

            users.AddRange(existingUsers);
        }

        // 사용자 정보를 객체로 생성
        var newUser = new UserInfo
        {
            Username = RegisterUsername,
            Password = RegisterPassword,
            Email = RegisterEmail,
            RegistrationDate = DateTime.Now
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
            ModelState.AddModelError("RegisterError", "사용자 정보를 저장하는 중 오류가 발생했습니다.");
            return Page();
        }

        // 임시로 등록 후 인덱스 페이지로 리디렉션
        return RedirectToPage("/Index");
    }

    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
