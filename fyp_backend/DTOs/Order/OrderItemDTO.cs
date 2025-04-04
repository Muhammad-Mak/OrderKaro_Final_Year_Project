namespace FYP_Backend.DTOs.Order
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string SpecialInstructions { get; set; } = string.Empty;
    }
}
