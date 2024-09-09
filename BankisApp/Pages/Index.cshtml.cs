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
            // �α��� ��Ű Ȯ��
            if (Request.Cookies["LoginCookie"] != null)
            {
                // ��Ű�� ������ main �������� �����̷�Ʈ
                return RedirectToPage("/Main");
            }
            else
            {
                // ��Ű�� ������ login �������� �����̷�Ʈ
                return RedirectToPage("/Account/Login");
            }
        }
    }
}
