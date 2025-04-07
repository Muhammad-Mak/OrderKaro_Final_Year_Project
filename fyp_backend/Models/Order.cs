namespace FYP_Backend.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string OrderType { get; set; } = null!; // Pickup / Table
        public int? TableNumber { get; set; } // Nullable if Pickup
        public string PaymentIntentId { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending / Cancelled / Completed
        public string PaymentStatus { get; set; } = "Pending"; // Pending / Succeeded / Failed
        public DateTime OrderDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
