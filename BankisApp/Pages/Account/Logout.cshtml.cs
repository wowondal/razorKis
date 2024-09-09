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
            // �α��� ��Ű ����
            Response.Cookies.Delete("LoginCookie");

            // �α��� �������� �����̷�Ʈ
            return RedirectToPage("/Account/Login");
        }
    }
}
