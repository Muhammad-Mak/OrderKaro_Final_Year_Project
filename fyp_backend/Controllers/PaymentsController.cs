using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: api/payments
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            // Initialize Stripe using the secret key from configuration
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        // ---------------- CREATE STRIPE PAYMENT INTENT ----------------
        // POST: api/payments/create-payment-intent?orderId=123
        // Role: Customer, Staff, Admin
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found.");

            // Convert total to paisa (minor unit)
            var amount = (int)(order.TotalAmount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "pkr",
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.OrderId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Save the intent ID to the order for later confirmation
            order.PaymentIntentId = paymentIntent.Id;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                clientSecret = paymentIntent.ClientSecret,
                paymentIntentId = paymentIntent.Id
            });
        }

        // ---------------- CONFIRM STRIPE PAYMENT ----------------
        // POST: api/payments/confirm?paymentIntentId=pi_123
        // Simulates confirmation (in production, use Stripe webhooks)
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment(string paymentIntentId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            if (order == null)
                return NotFound("Order not found for this payment.");

            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

            if (intent.Status == "succeeded")
            {
                order.PaymentStatus = "Succeeded";
                order.Status = "Pending";
            }
            else
            {
                order.PaymentStatus = "Failed";
                order.Status = "Cancelled";
            }

            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = order.Status,
                paymentStatus = order.PaymentStatus
            });
        }

        // ---------------- PAY USING BALANCE (RFID) ----------------
        // POST: api/payments/pay-with-balance?orderId=123
        // Deducts from user's balance if sufficient
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost("pay-with-balance")]
        public async Task<IActionResult> PayWithBalance(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found.");

            if (order.User.Balance < order.TotalAmount)
            {
                order.PaymentStatus = "Failed";
                order.Status = "Cancelled";
                return BadRequest("Insufficient balance.");
            }

            order.User.Balance -= order.TotalAmount;
            order.PaymentStatus = "Succeeded";
            order.Status = "Pending";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = order.Status,
                paymentStatus = order.PaymentStatus,
                newBalance = order.User.Balance
            });
        }
    }
}
