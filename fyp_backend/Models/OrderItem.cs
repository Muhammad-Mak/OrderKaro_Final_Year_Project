using FYP_Backend.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    public string SpecialInstructions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Order? Order { get; set; }
    public MenuItem? MenuItem { get; set; }
}
