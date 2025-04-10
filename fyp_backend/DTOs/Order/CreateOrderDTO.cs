namespace FYP_Backend.DTOs.Order
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; }
        public string OrderType { get; set; } = null!;
        public string? DeliveryLocation { get; set; } // now nullable
        public string PaymentIntentId { get; set; } = null!;
        public DateTime? ScheduledTime { get; set; }
        public List<CreateOrderItemDTO> OrderItems { get; set; } = new();
    }
}
