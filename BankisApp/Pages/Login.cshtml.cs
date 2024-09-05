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
        // �ʱ�ȭ �ڵ�
    }

    public IActionResult OnPost()
    {
    
        // ������ �α��� ���� ����
        if (Username == "admin" && Password == "password")
        {
            // �α��� ���� �� ��Ű ����
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30)
            };
            Response.Cookies.Append("LoginCookie", Username, options);

            // main �������� �����̷�Ʈ
            return RedirectToPage("/Main");
        }

        // �α��� ���� ��
        ErrorMessage = "Invalid login attempt.";
        return Page();
    }

    public IActionResult OnPostRegister()
    {
        // ��� ������ ���⿡ �߰��մϴ�.
        // ��: �����ͺ��̽��� ����� ���� ����

        // �ӽ÷� ��� �� �ε��� �������� ���𷺼�
        return RedirectToPage("/Index");
    }
}
