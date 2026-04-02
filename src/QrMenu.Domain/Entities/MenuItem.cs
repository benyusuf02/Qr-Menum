namespace QrMenu.Domain.Entities;

public class MenuItem
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Badges { get; set; }   // JSON array: ["popular","vegan"]
    public int SortOrder { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<MenuItemTranslation> Translations { get; set; } = new List<MenuItemTranslation>();
}