using Microsoft.EntityFrameworkCore;
using QrMenu.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QrMenu.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemTranslation> MenuItemTranslations => Set<MenuItemTranslation>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<MenuItemTranslation>()
          .HasKey(t => new { t.MenuItemId, t.LanguageCode });

        mb.Entity<Tenant>()
          .HasIndex(t => t.Slug).IsUnique();

        mb.Entity<MenuItem>()
          .Property(p => p.Price)
          .HasColumnType("decimal(18,2)");

        mb.Entity<User>()
          .HasIndex(u => u.Email).IsUnique();
    }
}