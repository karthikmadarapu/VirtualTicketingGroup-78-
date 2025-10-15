using System.ComponentModel.DataAnnotations;
namespace VirtualTicketing.Models;

public class Purchase
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string BuyerName { get; set; } = "";

    [EmailAddress, StringLength(100)]
    public string? BuyerEmail { get; set; }

    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}