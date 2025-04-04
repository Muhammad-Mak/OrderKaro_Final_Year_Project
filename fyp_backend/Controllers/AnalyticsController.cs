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

        [Authorize(Roles = "Admin")]
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
    }
}
