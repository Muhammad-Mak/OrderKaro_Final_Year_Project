using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        // POST: api/payments/create-payment-intent
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound("Order not found.");

            // Calculate total amount in cents
            var amount = (int)(order.TotalAmount * 100); // e.g., PKR 20.50 becomes 2050

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

            // Save intent ID in order
            order.PaymentIntentId = paymentIntent.Id;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                clientSecret = paymentIntent.ClientSecret,
                paymentIntentId = paymentIntent.Id
            });
        }

        // POST: api/payments/confirm
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment(string paymentIntentId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            if (order == null)
                return NotFound("Order not found for this payment.");

            // Simulate confirmation (in real Stripe, use webhooks)
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

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

            return Ok(new
            {
                status = order.Status,
                paymentStatus = order.PaymentStatus
            });
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("pay-with-balance")]
        public async Task<IActionResult> PayWithBalance(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return NotFound("Order not found.");
            if (order.User.Balance < order.TotalAmount)
                return BadRequest("Insufficient balance.");

            order.User.Balance -= order.TotalAmount;
            order.PaymentStatus = "Succeeded";
            order.Status = "Completed";
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
