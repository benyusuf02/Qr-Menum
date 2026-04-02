using Microsoft.EntityFrameworkCore;
using QrMenu.Domain.Entities;
using QrMenu.Infrastructure.Persistence;
using QrMenu.Infrastructure.Wolvox;
using QrMenu.Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var wolvoxOpts = builder.Configuration
    .GetSection("WolvoxSdk")
    .Get<WolvoxSdkOptions>()!;

builder.Services.AddHttpClient();
builder.Services.AddSingleton(wolvoxOpts);
builder.Services.AddScoped<WolvoxSdkService>();
builder.Services.AddScoped<QrCodeService>(sp =>
    new QrCodeService(sp.GetRequiredService<IWebHostEnvironment>().WebRootPath));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", p => p.RequireRole("SuperAdmin"));
    options.AddPolicy("TenantAdmin", p => p.RequireRole("TenantAdmin"));
});

builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

// Super admin yoksa oluştur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any(u => u.Role == "SuperAdmin"))
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("qrmenu2024")));
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            TenantId = null,
            Email = "admin@qrmenu.com",
            PasswordHash = hash,
            Role = "SuperAdmin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
