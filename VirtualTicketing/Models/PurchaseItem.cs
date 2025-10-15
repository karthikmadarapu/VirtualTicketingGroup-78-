
using System.ComponentModel.DataAnnotations.Schema;
namespace VirtualTicketing.Models;

public class PurchaseItem
{
    // Composite key will be configured via Fluent API
    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    // Extra payload on the link
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "numeric(10,2)")]
    public decimal UnitPrice { get; set; }
}