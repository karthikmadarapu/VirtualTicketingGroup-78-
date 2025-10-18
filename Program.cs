using Microsoft.EntityFrameworkCore;
using VirtualTicketing.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure PostgreSQL database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Add MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Apply migrations automatically (optional)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   // context.Database.Migrate(); // applies migrations if not applied
    DbInitializer.Initialize(context);
}

// ✅ Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();