using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FYP_Backend.Models;
using FYP_Backend.DTOs.Order;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/orders/user/5
        [Authorize(Roles = "Customer")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersForUser(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<List<OrderDTO>>(orders));
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            return Ok(_mapper.Map<OrderDTO>(order));
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            // Handle scheduled time logic
            if (dto.ScheduledTime == null)
            {
                dto.ScheduledTime = DateTime.UtcNow;
            }
            else if (dto.ScheduledTime < DateTime.UtcNow.AddMinutes(-1))
            {
                return BadRequest("Scheduled time must be now or in the future.");
            }

            if (dto.OrderType == "Delivery" && string.IsNullOrWhiteSpace(dto.DeliveryLocation))
            {
                return BadRequest("Please provide a valid delivery location for delivery orders.");
            }

            var order = _mapper.Map<Order>(dto);
            order.OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            order.Status = "Pending";
            order.TotalAmount = 0;
            order.OrderDate = DateTime.UtcNow;
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.ScheduledTime = dto.ScheduledTime;

            foreach (var item in order.OrderItems!)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                if (menuItem == null) return BadRequest($"MenuItem ID {item.MenuItemId} not found.");

                item.UnitPrice = menuItem.Price;
                item.SubTotal = item.UnitPrice * item.Quantity;
                order.TotalAmount += item.SubTotal;
                item.CreatedAt = DateTime.UtcNow;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var result = await _context.Orders
                .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, _mapper.Map<OrderDTO>(result));
        }
    }
}