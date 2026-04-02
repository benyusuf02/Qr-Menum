using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;

namespace QrMenu.API.Pages.SuperAdmin;

[Authorize(Roles = "SuperAdmin")]
public class TenantRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int KategoriSayisi { get; set; }
    public int UrunSayisi { get; set; }
}

public class TenantsModel : PageModel
{
    private readonly AppDbContext _db;
    public List<TenantRow> Tenants { get; set; } = new();

    public TenantsModel(AppDbContext db) { _db = db; }

    public async Task OnGetAsync()
    {
        Tenants = await _db.Tenants
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TenantRow
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                PlanType = t.PlanType,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                KategoriSayisi = t.Restaurants.SelectMany(r => r.Categories).Count(),
                UrunSayisi = t.Restaurants.SelectMany(r => r.Categories).SelectMany(c => c.Items).Count()
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleAsync(Guid id)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant != null)
        {
            tenant.IsActive = !tenant.IsActive;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdatePlanAsync(Guid id, string planType)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant != null)
        {
            tenant.PlanType = planType;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
