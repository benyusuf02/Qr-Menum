namespace QrMenu.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BrandColor { get; set; } = "#0F6E56";
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}