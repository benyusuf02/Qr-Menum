using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;
using System.Security.Claims;

namespace QrMenu.API.Pages.Menu;

[Authorize(Roles = "TenantAdmin")]
public class ItemRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string? Badges { get; set; }
}

public class ItemsModel : PageModel
{
    private readonly AppDbContext _db;
    public List<ItemRow> Items { get; set; } = new();
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public ItemsModel(AppDbContext db) { _db = db; }

    public async Task<IActionResult> OnGetAsync(Guid catId)
    {
        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);

        // Kategorinin bu tenant'a ait olduğunu doğrula
        var cat = await _db.Categories
            .Include(c => c.Restaurant)
            .FirstOrDefaultAsync(c => c.Id == catId && c.Restaurant.TenantId == tenantId);

        if (cat == null) return RedirectToPage("/Menu/Categories");

        CategoryId = catId;
        CategoryName = cat.Name;

        Items = await _db.MenuItems
            .Where(i => i.CategoryId == catId)
            .OrderBy(i => i.SortOrder)
            .Select(i => new ItemRow
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                IsAvailable = i.IsAvailable,
                Badges = i.Badges
            })
            .ToListAsync();

        return Page();
    }
}
