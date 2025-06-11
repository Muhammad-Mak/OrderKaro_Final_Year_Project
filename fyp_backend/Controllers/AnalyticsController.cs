using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("totals")]
        public async Task<IActionResult> GetTotals()
        {
            var totalOrders = await _context.Orders.CountAsync();

            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Completed")
                .SumAsync(o => o.TotalAmount);

            return Ok(new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue
            });
        }

        [Authorize(Roles = "Admin, Customer, Staff")]
        [HttpGet("popular-items")]
        public async Task<IActionResult> GetTopOrderedItems()
        {
            var topItems = await _context.MenuItems
                .OrderByDescending(m => m.OrderCount)
                .Take(6)
                .Select(m => new { m.Name, m.OrderCount })
                .ToListAsync();

            return Ok(topItems);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("orders-per-week")]
        public async Task<IActionResult> GetOrdersPerWeek()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-6);
            var endDate = DateTime.UtcNow.Date.AddDays(1);

            var data = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        [Authorize(Roles = "Admin")]
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

      
        //[Authorize(Roles = "Admin")]
        [HttpGet("sales-history")]
        public async Task<IActionResult> GetSalesHistory()
        {
            var history = await _context.OrderItems
                .Include(oi => oi.Order)
                .GroupBy(oi => new { oi.MenuItemId, Date = oi.Order.OrderDate.Date })
                .Select(g => new
                {
                    MenuItemId = g.Key.MenuItemId,
                    Date = g.Key.Date,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}
