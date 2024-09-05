using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class MainModel : PageModel
{
    public string Username { get; set; }
    public void OnGet()
    {
        // 초기화 코드
        Username = Request.Cookies["LoginCookie"].ToString();

    }
    public IActionResult OnPostLogout()
    {
        // 로그인 쿠키 삭제
        Response.Cookies.Delete("LoginCookie");

        // 로그인 페이지로 리다이렉트
        return RedirectToPage("/Login");
    }
}