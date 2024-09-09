using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankisApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // 로그인 쿠키 확인
            if (Request.Cookies["LoginCookie"] != null)
            {
                // 쿠키가 있으면 main 페이지로 리다이렉트
                return RedirectToPage("/Main");
            }
            else
            {
                // 쿠키가 없으면 login 페이지로 리다이렉트
                return RedirectToPage("/Account/Login");
            }
        }
    }
}
