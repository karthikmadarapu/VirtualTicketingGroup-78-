using System.ComponentModel.DataAnnotations;

namespace VirtualTicketing.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        // FK
        public int PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
    }
}