namespace FYP_Backend.DTOs.Order
{
    public class CreateOrderItemDTO
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; } = string.Empty;
    }
}
