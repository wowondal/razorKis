using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BankisApp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            // 로그인 쿠키 삭제
            Response.Cookies.Delete("LoginCookie");

            // 로그인 페이지로 리다이렉트
            return RedirectToPage("/Account/Login");
        }
    }
}
