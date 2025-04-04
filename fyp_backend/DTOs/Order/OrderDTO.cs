namespace FYP_Backend.DTOs.Order
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string OrderType { get; set; } = null!; // Pickup or Table
        public int? TableNumber { get; set; }
        public string Status { get; set; } = null!; // Pending, Completed, Cancelled
        public string PaymentIntentId { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }
}
