using VirtualTicketing.Data;
using VirtualTicketing.Models;

namespace VirtualTicketing.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext db)
    {
        db.Database.EnsureCreated(); // safe with migrations

        if (!db.Categories.Any())
        {
            var cat = new Category { Name = "Concerts" };
            db.Categories.Add(cat);
            db.SaveChanges();

            db.Events.Add(new Event
            {
                Title = "Freshers Night",
                Description = "Welcome concert",
                StartAt = DateTime.UtcNow.AddDays(7),
                CategoryId = cat.Id
            });
            db.SaveChanges();
        }
    }
}