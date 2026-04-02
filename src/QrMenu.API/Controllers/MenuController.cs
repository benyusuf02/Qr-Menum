using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenu.Infrastructure.Persistence;


using QrMenu.Infrastructure.Persistence;

namespace QrMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {

        private readonly AppDbContext _db;
        public MenuController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetMenu(string slug, [FromQuery] string lang = "tr")
        {
            var tenant = await _db.Tenants.Where(t => t.Slug == slug && t.IsActive)
                 .Include(r => r.Restaurants)
                 .ThenInclude(c => c.Categories.Where(c => c.IsActive))
                 .ThenInclude(i => i.Items.Where(i => i.IsAvailable))
                 .ThenInclude(i => i.Translations)
                 .FirstOrDefaultAsync();

            if (tenant == null)
                return NotFound(new { message = "Menü Bulunamadı" });

            var restaurant = tenant.Restaurants.FirstOrDefault();
            if (restaurant == null)
                return NotFound(new { message = "Restoran Bulunamadı" });
            var result = new
            {
                restaurantName = restaurant.Name,
                brandColor = restaurant.BrandColor,
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
                                       // İstenen dilde çeviri var mı?
                                       var translation = i.Translations
                                           .FirstOrDefault(t => t.LanguageCode == lang);

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
                                               : System.Text.Json.JsonSerializer
                                                   .Deserialize<List<string>>(i.Badges)
                                       };
                                   })
                           })
            };

            return Ok(result);
        }
    }
}
    