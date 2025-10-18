using VirtualTicketing.Models;

namespace VirtualTicketing.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created (for In-Memory DB)
            context.Database.EnsureCreated();

            // ✅ Seed Categories if not present
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Concerts" },
                    new Category { Name = "Sports" },
                    new Category { Name = "Theatre" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // ✅ Seed Events if not present
            if (!context.Events.Any())
            {
                var concerts = context.Categories.First(c => c.Name == "Concerts");
                var sports = context.Categories.First(c => c.Name == "Sports");
                var theatre = context.Categories.First(c => c.Name == "Theatre");

                var events = new[]
                {
                    new Event
                    {
                        Name = "Coldplay Live",
                        Location = "Toronto",
                        Date = new DateTime(2025, 12, 5),
                        CategoryId = concerts.Id,
                        Description = "An electrifying live performance by Coldplay featuring hits from all albums.",
                        Organizer = "Live Nation",
                        TeamInfo = null
                    },
                    new Event
                    {
                        Name = "NBA Finals",
                        Location = "Scotiabank Arena",
                        Date = new DateTime(2025, 6, 12),
                        CategoryId = sports.Id,
                        Description = "The championship series of the NBA — East vs West showdown.",
                        Organizer = "NBA",
                        TeamInfo = "Toronto Raptors current lineup"
                    },
                    new Event
                    {
                        Name = "Hamilton Musical",
                        Location = "Mirvish Theatre",
                        Date = new DateTime(2025, 8, 20),
                        CategoryId = theatre.Id,
                        Description = "A Broadway musical blending hip-hop, R&B, and history to tell the story of Alexander Hamilton.",
                        Organizer = "Mirvish Productions",
                        TeamInfo = null
                    },
                    new Event
                    {
                        Name = "Jays Game",
                        Location = "Rogers Centre",
                        Date = new DateTime(2025, 7, 10),
                        CategoryId = sports.Id,
                        Description = "Toronto Blue Jays vs New York Yankees — MLB regular season game.",
                        Organizer = "MLB Canada",
                        TeamInfo = "Toronto Blue Jays current XI"
                    }
                };

                context.Events.AddRange(events);
                context.SaveChanges();
            }
        }
    }
}
