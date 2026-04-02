using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QrMenu.API.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? SuccessMessage { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public LoginModel(AppDbContext db) { _db = db; }

    public void OnGet(string? registered)
    {
        if (registered == "1")
            SuccessMessage = "Hesabınız oluşturuldu. Giriş yapabilirsiniz.";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Password)));

        var user = await _db.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == hash && u.IsActive);

        if (user == null)
        {
            ErrorMessage = "E-posta veya şifre hatalı.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role),
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new("TenantId", user.TenantId.Value.ToString()));
            claims.Add(new("TenantName", user.Tenant?.Name ?? string.Empty));
        }

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("Cookies", principal);

        return user.Role == "SuperAdmin"
            ? RedirectToPage("/SuperAdmin/Dashboard")
            : RedirectToPage("/Dashboard/Index");
    }
}
