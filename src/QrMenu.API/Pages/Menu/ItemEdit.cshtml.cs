using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QrMenu.Domain.Entities;
using QrMenu.Infrastructure.Persistence;
using System.Security.Claims;
using System.Text.Json;

namespace QrMenu.API.Pages.Menu;

[Authorize(Roles = "TenantAdmin")]
public class ItemEditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    [BindProperty] public Guid? Id { get; set; }
    [BindProperty] public Guid CategoryId { get; set; }
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string? Description { get; set; }
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public int SortOrder { get; set; }
    [BindProperty] public bool IsAvailable { get; set; } = true;
    [BindProperty] public List<string> SelectedBadges { get; set; } = new();
    [BindProperty] public IFormFile? ImageFile { get; set; }
    public string? CurrentImageUrl { get; set; }

    public bool IsNew => Id == null || Id == Guid.Empty;

    public ItemEditModel(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> OnGetAsync(Guid? id, Guid catId)
    {
        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);

        // Kategorinin bu tenant'a ait olduğunu doğrula
        var cat = await _db.Categories
            .Include(c => c.Restaurant)
            .FirstOrDefaultAsync(c => c.Id == catId && c.Restaurant.TenantId == tenantId);

        if (cat == null) return RedirectToPage("/Menu/Categories");

        CategoryId = catId;
        if (id == null) return Page();

        var item = await _db.MenuItems
            .FirstOrDefaultAsync(i => i.Id == id && i.CategoryId == catId);

        if (item == null) return RedirectToPage("/Menu/Items", new { catId });

        Id = item.Id;
        Name = item.Name;
        Description = item.Description;
        Price = item.Price;
        SortOrder = item.SortOrder;
        IsAvailable = item.IsAvailable;
        CurrentImageUrl = item.ImageUrl;
        SelectedBadges = string.IsNullOrEmpty(item.Badges)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(item.Badges) ?? new();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var tenantId = Guid.Parse(User.FindFirstValue("TenantId")!);

        // Kategori ownership doğrula
        var cat = await _db.Categories
            .Include(c => c.Restaurant)
            .FirstOrDefaultAsync(c => c.Id == CategoryId && c.Restaurant.TenantId == tenantId);

        if (cat == null) return RedirectToPage("/Menu/Categories");

        var badges = JsonSerializer.Serialize(SelectedBadges);

        // Resim yükleme
        string? uploadedImageUrl = null;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (allowedTypes.Contains(ImageFile.ContentType))
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "images", "items");
                Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using var stream = System.IO.File.Create(filePath);
                await ImageFile.CopyToAsync(stream);
                uploadedImageUrl = $"/images/items/{fileName}";
            }
        }

        if (IsNew)
        {
            _db.MenuItems.Add(new MenuItem
            {
                Id = Guid.NewGuid(),
                CategoryId = CategoryId,
                Name = Name,
                Description = Description,
                Price = Price,
                SortOrder = SortOrder,
                IsAvailable = IsAvailable,
                Badges = badges,
                ImageUrl = uploadedImageUrl
            });
        }
        else
        {
            var item = await _db.MenuItems
                .FirstOrDefaultAsync(i => i.Id == Id && i.CategoryId == CategoryId);

            if (item != null)
            {
                item.Name = Name;
                item.Description = Description;
                item.Price = Price;
                item.SortOrder = SortOrder;
                item.IsAvailable = IsAvailable;
                item.Badges = badges;
                if (uploadedImageUrl != null)
                    item.ImageUrl = uploadedImageUrl;
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("/Menu/Items", new { catId = CategoryId });
    }
}
