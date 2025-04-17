using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route becomes: api/payments
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;           // Database context
        private readonly IConfiguration _configuration;   // Configuration to access Stripe keys

        public PaymentsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            // Set the Stripe secret key from appsettings.json
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        // POST: api/payments/create-payment-intent
        // Creates a Stripe payment intent for a given order
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(int orderId)
        {
            // Find the order and include order items
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found.");

            // Stripe accepts amount in the smallest currency unit (e.g. paisa)
            var amount = (int)(order.TotalAmount * 100); // e.g., 20.50 PKR becomes 2050

            // Configure Stripe payment intent options
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "pkr",
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.OrderId.ToString() } // Optional metadata for tracking
                }
            };

            // Create payment intent through Stripe
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Save the Stripe PaymentIntent ID to the order
            order.PaymentIntentId = paymentIntent.Id;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Return client secret and intent ID to the frontend
            return Ok(new
            {
                clientSecret = paymentIntent.ClientSecret,
                paymentIntentId = paymentIntent.Id
            });
        }

        // POST: api/payments/confirm
        // Confirms payment after Stripe processes it (simulated; in real life, use webhooks)
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment(string paymentIntentId)
        {
            // Find order using the saved PaymentIntent ID
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            if (order == null)
                return NotFound("Order not found for this payment.");

            // Simulate payment verification (use Stripe webhooks in production)
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

            // Update order status based on Stripe payment status
            if (intent.Status == "succeeded")
            {
                order.PaymentStatus = "Succeeded";
                order.Status = "Completed";
            }
            else
            {
                order.PaymentStatus = "Failed";
                order.Status = "Cancelled";
            }

            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Return updated payment and order status
            return Ok(new
            {
                status = order.Status,
                paymentStatus = order.PaymentStatus
            });
        }

        // POST: api/payments/pay-with-balance
        // Processes payment using the customer's RFID card balance
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("pay-with-balance")]
        public async Task<IActionResult> PayWithBalance(int orderId)
        {
            // Fetch the order and include the related user (for balance)
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found.");

            // Check if user's balance is enough to cover the order
            if (order.User.Balance < order.TotalAmount)
                return BadRequest("Insufficient balance.");

            // Deduct balance and mark order as paid
            order.User.Balance -= order.TotalAmount;
            order.PaymentStatus = "Succeeded";
            order.Status = "Completed";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return updated order status and user's new balance
            return Ok(new
            {
                status = order.Status,
                paymentStatus = order.PaymentStatus,
                newBalance = order.User.Balance
            });
        }
    }
}
