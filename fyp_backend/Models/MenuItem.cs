using FYP_Backend.Models;

public class MenuItem
{
    public int MenuItemId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public int OrderCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Category? Category { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}
