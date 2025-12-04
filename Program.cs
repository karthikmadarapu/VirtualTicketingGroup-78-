using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VirtualTicketing.Data;
using VirtualTicketing.Models;


var builder = WebApplication.CreateBuilder(args);





// ================================
// 🔹 1. Configure Database (PostgreSQL)
// ================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ================================
// 🔹 2. Configure Identity (User + Roles + Email Confirmations)
// ================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ================================
// 🔹 3. Configure Serilog
// ================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("wwwroot/logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ================================
// 🔹 4. MVC + Razor
// ================================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ================================
// 🔹 5. Build App
// ================================
var app = builder.Build();

// ================================
// 🔹 6. Apply Migrations & Seed Roles
// ================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Run role seeding
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error seeding database or applying migrations");
    }
}

// ================================
// 🔹 7. Configure Middleware Pipeline
// ================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ================================
// 🔹 8. Routes
// ================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ================================
// 🔹 9. Run App
// ================================
try
{
    Log.Information("Starting VirtualTicketing Application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
