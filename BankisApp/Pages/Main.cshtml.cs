using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class MainModel : PageModel
{
    public string Username { get; set; }
    public void OnGet()
    {
        // �ʱ�ȭ �ڵ�
        Username = Request.Cookies["LoginCookie"].ToString();

    }
    public IActionResult OnPostLogout()
    {
        // �α��� ��Ű ����
        Response.Cookies.Delete("LoginCookie");

        // �α��� �������� �����̷�Ʈ
        return RedirectToPage("/Login");
    }
}