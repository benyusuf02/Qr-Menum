using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;

namespace QrMenu.API.Pages.SuperAdmin;

[Authorize(Roles = "SuperAdmin")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;

    public int ToplamTenant { get; set; }
    public int AktifTenant { get; set; }
    public int ToplamKullanici { get; set; }
    public int ToplamUrun { get; set; }

    public DashboardModel(AppDbContext db) { _db = db; }

    public async Task OnGetAsync()
    {
        ToplamTenant = await _db.Tenants.CountAsync();
        AktifTenant = await _db.Tenants.CountAsync(t => t.IsActive);
        ToplamKullanici = await _db.Users.CountAsync(u => u.Role == "TenantAdmin");
        ToplamUrun = await _db.MenuItems.CountAsync();
    }
}
