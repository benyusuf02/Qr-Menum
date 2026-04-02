using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;

namespace QrMenu.API.Pages;

public class MModel : PageModel
{
    private readonly AppDbContext _db;

    public string RestaurantDataJson { get; set; } = "null";
    public string RestaurantName { get; set; } = string.Empty;
    public string BrandColor { get; set; } = "#0f6e56";
    public string TableInfo { get; set; } = string.Empty;

    public MModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGetAsync(string slug, [FromQuery] string table, [FromQuery] string lang = "tr")
    {
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();

        var tenant = await _db.Tenants.Where(t => t.Slug == slug && t.IsActive)
             .Include(r => r.Restaurants)
             .ThenInclude(c => c.Categories.Where(c => c.IsActive))
             .ThenInclude(i => i.Items.Where(i => i.IsAvailable))
             .ThenInclude(i => i.Translations)
             .FirstOrDefaultAsync();

        if (tenant == null) return NotFound("Menü Bulunamadı");

        var restaurant = tenant.Restaurants.FirstOrDefault();
        if (restaurant == null) return NotFound("Restoran Bulunamadı");

        RestaurantName = restaurant.Name;
        BrandColor = string.IsNullOrEmpty(restaurant.BrandColor) ? "#0f6e56" : restaurant.BrandColor;
        TableInfo = string.IsNullOrWhiteSpace(table) ? "" : $"Masa {table}";

        var result = new
        {
            restaurantName = restaurant.Name,
            brandColor = BrandColor,
            logoUrl = restaurant.LogoUrl,
            categories = restaurant.Categories
                       .OrderBy(c => c.SortOrder)
                       .Select(c => new
                       {
                           id = c.Id,
                           name = c.Name,
                           items = c.Items
                               .OrderBy(i => i.SortOrder)
                               .Select(i =>
                               {
                                   var translation = i.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                                   return new
                                   {
                                       id = i.Id,
                                       name = translation?.Name ?? i.Name,
                                       description = translation?.Description ?? i.Description,
                                       price = i.Price,
                                       imageUrl = i.ImageUrl,
                                       isAvailable = i.IsAvailable,
                                       badges = string.IsNullOrEmpty(i.Badges)
                                           ? new List<string>()
                                           : System.Text.Json.JsonSerializer.Deserialize<List<string>>(i.Badges)
                                   };
                               })
                       })
        };

        RestaurantDataJson = System.Text.Json.JsonSerializer.Serialize(result);
        return Page();
    }
}
