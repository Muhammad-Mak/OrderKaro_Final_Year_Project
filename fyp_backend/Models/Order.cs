namespace FYP_Backend.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string OrderType { get; set; } = null!; // "Pickup" or "Delivery"
        public string? DeliveryLocation { get; set; } // replaces TableNumber, now nullable
        public string PaymentIntentId { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Unpaid";
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ScheduledTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
