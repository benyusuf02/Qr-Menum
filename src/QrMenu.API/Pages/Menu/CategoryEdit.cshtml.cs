using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Domain.Entities;
using QrMenu.Infrastructure.Persistence;
using System.Security.Claims;

namespace QrMenu.API.Pages.Menu;

[Authorize(Roles = "TenantAdmin")]
public class CategoryEditModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty] public Guid? Id { get; set; }
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public int SortOrder { get; set; }
    [BindProperty] public bool IsActive { get; set; } = true;

    public bool IsNew => Id == null || Id == Guid.Empty;

    public CategoryEditModel(AppDbContext db) { _db = db; }

    public async Task OnGetAsync(Guid? id)
    {
        if (id == null) return;

        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);
        var cat = await _db.Categories
            .Include(c => c.Restaurant)
            .FirstOrDefaultAsync(c => c.Id == id && c.Restaurant.TenantId == tenantId);

        if (cat == null) return;

        Id = cat.Id;
        Name = cat.Name;
        SortOrder = cat.SortOrder;
        IsActive = cat.IsActive;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);
        var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.TenantId == tenantId);
        if (restaurant == null) return RedirectToPage("/Menu/Categories");

        if (IsNew)
        {
            _db.Categories.Add(new Category
            {
                Id = Guid.NewGuid(),
                RestaurantId = restaurant.Id,
                Name = Name,
                SortOrder = SortOrder,
                IsActive = IsActive
            });
        }
        else
        {
            var cat = await _db.Categories
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(c => c.Id == Id && c.Restaurant.TenantId == tenantId);

            if (cat != null)
            {
                cat.Name = Name;
                cat.SortOrder = SortOrder;
                cat.IsActive = IsActive;
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("/Menu/Categories");
    }
}
