using System.ComponentModel.DataAnnotations;

namespace VirtualTicketing.Models;

public class Event
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime StartAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? EndAt { get; set; }

    // FK
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigation
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}