using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
        // 초기화 코드
    }

    public IActionResult OnPost()
    {
    
        // 간단한 로그인 검증 예제
        if (Username == "admin" && Password == "password")
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

        // 로그인 실패 시
        ErrorMessage = "Invalid login attempt.";
        return Page();
    }

    public IActionResult OnPostRegister()
    {
        // 등록 로직을 여기에 추가합니다.
        // 예: 데이터베이스에 사용자 정보 저장

        // 임시로 등록 후 인덱스 페이지로 리디렉션
        return RedirectToPage("/Index");
    }
}
