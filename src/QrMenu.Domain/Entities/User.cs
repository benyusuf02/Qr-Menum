namespace QrMenu.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }       // null = SuperAdmin
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;   // SHA-256 hex
    public string Role { get; set; } = "TenantAdmin";          // "SuperAdmin" | "TenantAdmin"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tenant? Tenant { get; set; }
}
