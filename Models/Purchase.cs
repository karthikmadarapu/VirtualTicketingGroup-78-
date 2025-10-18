using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtualTicketing.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string GuestName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string GuestEmail { get; set; } = string.Empty;

        // ✅ Store UTC instead of local time to prevent PostgreSQL errors
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public decimal TotalCost { get; set; }

        // Navigation
        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
}