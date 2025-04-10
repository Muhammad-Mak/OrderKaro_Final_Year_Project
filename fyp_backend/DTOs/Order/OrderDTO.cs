namespace FYP_Backend.DTOs.Order
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string OrderType { get; set; } = null!;
        public string? DeliveryLocation { get; set; } // now nullable
        public string Status { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string PaymentIntentId { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }
}
