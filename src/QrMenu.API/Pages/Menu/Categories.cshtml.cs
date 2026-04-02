using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;
using System.Security.Claims;

namespace QrMenu.API.Pages.Menu;

[Authorize(Roles = "TenantAdmin")]
public class CategoryRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ItemCount { get; set; }
}

public class CategoriesModel : PageModel
{
    private readonly AppDbContext _db;
    public List<CategoryRow> Categories { get; set; } = new();

    public CategoriesModel(AppDbContext db) { _db = db; }

    public async Task OnGetAsync()
    {
        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);

        Categories = await _db.Categories
            .Where(c => c.Restaurant.TenantId == tenantId)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryRow
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                ItemCount = c.Items.Count()
            })
            .ToListAsync();
    }
}
