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
                // �α��� ���� �� ��Ű ����
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Append("LoginCookie", Username, options);

                // main �������� �����̷�Ʈ
                return RedirectToPage("/Main");
            }
        }

        // �α��� ���� ��
        ModelState.AddModelError("LoginError", "Invalid login attempt.");
        return Page();
    }

    public IActionResult OnPostRegister()
    {
        ModelState.ClearValidationState(nameof(LoginError));

        string filePath = "./user.txt";
        List<UserInfo> users = new List<UserInfo>();

        // ����� �̸��� �̹� �����ϴ��� Ȯ��
        if (System.IO.File.Exists(filePath))
        {
            var existingUsers = System.IO.File.ReadAllLines(filePath)
                .Select(line => JsonSerializer.Deserialize<UserInfo>(line));

            if (existingUsers.Any(user => user.Username == RegisterUsername))
            {
                ModelState.AddModelError("RegisterError", "�̹� �����ϴ� ����� �̸��Դϴ�.");
                return Page();
            }

            users.AddRange(existingUsers);
        }

        // ����� ������ ��ü�� ����
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
            // ���� ���� ���� ó��
            ModelState.AddModelError("RegisterError", "����� ������ �����ϴ� �� ������ �߻��߽��ϴ�.");
            return Page();
        }

        // �ӽ÷� ��� �� �ε��� �������� ���𷺼�
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
