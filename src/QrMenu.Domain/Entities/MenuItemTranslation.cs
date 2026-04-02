namespace QrMenu.Domain.Entities;

public class MenuItemTranslation
{
    public Guid MenuItemId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;  // "tr","en","ar"
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
}