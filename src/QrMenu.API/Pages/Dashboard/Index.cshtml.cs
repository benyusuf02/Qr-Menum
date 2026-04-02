using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;
using QrMenu.Infrastructure.Services;
using System.Security.Claims;

namespace QrMenu.API.Pages.Dashboard;

[Authorize(Roles = "TenantAdmin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly QrCodeService _qrService;

    public int ToplamUrun { get; set; }
    public int ToplamKategori { get; set; }
    public int AktifUrun { get; set; }
    public string MenuUrl { get; set; } = string.Empty;
    public string QrImageUrl { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;

    public IndexModel(AppDbContext db, QrCodeService qrService)
    {
        _db = db;
        _qrService = qrService;
    }

    public async Task OnGetAsync()
    {
        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);

        ToplamUrun = await _db.MenuItems
            .Where(i => i.Category.Restaurant.TenantId == tenantId)
            .CountAsync();

        ToplamKategori = await _db.Categories
            .Where(c => c.Restaurant.TenantId == tenantId)
            .CountAsync();

        AktifUrun = await _db.MenuItems
            .Where(i => i.IsAvailable && i.Category.Restaurant.TenantId == tenantId)
            .CountAsync();

        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (tenant != null)
        {
            TenantSlug = tenant.Slug;
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            MenuUrl = $"{baseUrl}/m/{tenant.Slug}";
            QrImageUrl = _qrService.Generate(tenant.Slug, baseUrl);
        }
    }
}
