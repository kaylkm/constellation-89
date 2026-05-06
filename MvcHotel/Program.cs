using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MvcHotel.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Автостворення таблиці capsule_prices та seed при першому запуску
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
     await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS capsule_prices (
            id    SERIAL PRIMARY KEY,
            slug  VARCHAR(50) NOT NULL UNIQUE,
            price INTEGER NOT NULL
        )
    ");

    if (!await db.CapsulePrices.AnyAsync())
    {
        db.CapsulePrices.AddRange(
            new CapsulePrice { Slug = "single",  Price = 300  },
            new CapsulePrice { Slug = "singlew", Price = 350 },
            new CapsulePrice { Slug = "singlem", Price = 350 },
            new CapsulePrice { Slug = "double",  Price = 650 }
        );
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
