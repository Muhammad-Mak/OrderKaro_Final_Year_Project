using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    // Marks this class as an API controller
    [ApiController]
    // Route for this controller will be: api/analytics
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        // Database context to access the database
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // Only accessible to users with the "Admin" role
        [Authorize(Roles = "Admin")]
        // GET: api/analytics/totals
        [HttpGet("totals")]
        public async Task<IActionResult> GetTotals()
        {
            // Count total number of orders in the system
            var totalOrders = await _context.Orders.CountAsync();

            // Calculate the sum of total amounts for all completed orders
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Completed")
                .SumAsync(o => o.TotalAmount);

            // Return the totals as a JSON object
            return Ok(new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue
            });
        }

        // Only accessible to Admin users
        [Authorize(Roles = "Admin, Customer, Staff")]
        // GET: api/analytics/popular-items
        [HttpGet("popular-items")]
        public async Task<IActionResult> GetTopOrderedItems()
        {
            // Get top 6 menu items based on OrderCount in descending order
            var topItems = await _context.MenuItems
                .OrderByDescending(m => m.OrderCount)
                .Take(6)
                .Select(m => new { m.Name, m.OrderCount }) // Select only relevant fields
                .ToListAsync();

            // Return the list of top items
            return Ok(topItems);
        }
        [Authorize(Roles = "Admin")]
        // GET: api/analytics/orders-per-week
        [HttpGet("orders-per-week")]
        public async Task<IActionResult> GetOrdersPerWeek()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-6);
            var endDate = DateTime.UtcNow.Date.AddDays(1);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }
        [Authorize(Roles = "Admin")]
        // GET: api/analytics/order-type-ratio
        [HttpGet("order-type-ratio")]
        public async Task<IActionResult> GetOrderTypeRatio()
        {
            var pickupCount = await _context.Orders.CountAsync(o => o.OrderType == "Pickup");
            var deliveryCount = await _context.Orders.CountAsync(o => o.OrderType == "Delivery");

            return Ok(new
            {
                Pickup = pickupCount,
                Delivery = deliveryCount
            });
        }
    }
}
