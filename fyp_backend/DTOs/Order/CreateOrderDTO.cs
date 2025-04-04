namespace FYP_Backend.DTOs.Order
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; } // From logged-in user or mobile
        public string OrderType { get; set; } = null!;
        public int? TableNumber { get; set; } // nullable if OrderType is Pickup
        public List<CreateOrderItemDTO> OrderItems { get; set; } = new();
        public string PaymentIntentId { get; set; } = null!;
    }
}
