using System;

namespace VirtualTicketing.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);




        // Foreign key
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // 🆕 New fields for richer info
        public string? Description { get; set; }
        public string? Organizer { get; set; }
        public string? TeamInfo { get; set; }

        // 🆕 Ticket info fields
        public int AvailableTickets { get; set; } = 100;
        public decimal Price { get; set; } = 120.00m;
    }

}