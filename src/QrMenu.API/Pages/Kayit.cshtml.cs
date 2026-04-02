using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Domain.Entities;
using QrMenu.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QrMenu.API.Pages;

public class KayitModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty] public string RestoranAdi { get; set; } = string.Empty;
    [BindProperty] public string Slug { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public KayitModel(AppDbContext db) { _db = db; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        Slug = Slug.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(RestoranAdi) || string.IsNullOrWhiteSpace(Slug)
            || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Tüm alanlar zorunludur.";
            return Page();
        }

        if (!Regex.IsMatch(Slug, @"^[a-z0-9\-]+$"))
        {
            ErrorMessage = "Slug yalnızca küçük harf, rakam ve tire (-) içerebilir.";
            return Page();
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Şifre en az 6 karakter olmalıdır.";
            return Page();
        }

        if (await _db.Tenants.AnyAsync(t => t.Slug == Slug))
        {
            ErrorMessage = "Bu URL (slug) zaten kullanılıyor. Farklı bir şey deneyin.";
            return Page();
        }

        if (await _db.Users.AnyAsync(u => u.Email == Email))
        {
            ErrorMessage = "Bu e-posta adresi zaten kayıtlı.";
            return Page();
        }

        var tenantId = Guid.NewGuid();

        _db.Tenants.Add(new Tenant
        {
            Id = tenantId,
            Name = RestoranAdi,
            Slug = Slug,
            PlanType = "free",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        _db.Restaurants.Add(new Restaurant
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = RestoranAdi,
            IsActive = true
        });

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Password)));
        _db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = Email,
            PasswordHash = hash,
            Role = "TenantAdmin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return RedirectToPage("/Auth/Login", new { registered = "1" });
    }
}
