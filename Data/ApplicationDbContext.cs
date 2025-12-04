using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtualTicketing.Models;
using System;

namespace VirtualTicketing.Data
{
    // NOTE: Changed base class from DbContext → IdentityDbContext<ApplicationUser, IdentityRole, string>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

        public bool CanConnect() => Database.CanConnect();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: call base so Identity tables are configured
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data for Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Concerts" },
                new Category { Id = 2, Name = "Sports" },
                new Category { Id = 3, Name = "Theatre" }
            );

            // Seed data for Events
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Name = "Coldplay Live",
                    Location = "Toronto",
                    Date = new DateTime(2025, 12, 5, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = 1,
                    Price = 120
                },
                new Event
                {
                    Id = 2,
                    Name = "NBA Finals",
                    Location = "Scotiabank Arena",
                    Date = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = 2,
                    Price = 150
                },
                new Event
                {
                    Id = 3,
                    Name = "Hamilton Musical",
                    Location = "Mirvish Theatre",
                    Date = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = 3,
                    Price = 100
                },
                new Event
                {
                    Id = 4,
                    Name = "Jays Game",
                    Location = "Rogers Centre",
                    Date = new DateTime(2025, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = 2,
                    Price = 110
                }
            );
        }
    }
}
