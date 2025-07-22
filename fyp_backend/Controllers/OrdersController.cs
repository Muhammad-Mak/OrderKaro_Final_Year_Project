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
    [Route("api/[controller]")] // Route: api/orders
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ---------------- GET ALL ORDERS FOR A USER ----------------
        // GET: api/orders/user/{userId}
        // Role: Admin, Staff, Customer
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersForUser(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<List<OrderDTO>>(orders));
        }

        // ---------------- GET ALL ORDERS ----------------
        // GET: api/orders
        // Role: Admin, Staff
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<List<OrderDTO>>(orders));
        }

        // ---------------- GET ORDER BY ID ----------------
        // GET: api/orders/{id}
        // Role: Admin, Staff, Customer
        [Authorize(Roles = "Customer, Staff, Admin")]
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

        // ---------------- CREATE ORDER ----------------
        // POST: api/orders
        // Role: Admin, Staff, Customer
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            // Validate ScheduledTime
            if (dto.ScheduledTime == null)
            {
                dto.ScheduledTime = DateTime.UtcNow;
            }
            else if (dto.ScheduledTime < DateTime.UtcNow.AddMinutes(-1))
            {
                return BadRequest("Scheduled time must be now or in the future.");
            }

            // Require DeliveryLocation if delivery type
            if (dto.OrderType == "Delivery" && string.IsNullOrWhiteSpace(dto.DeliveryLocation))
            {
                return BadRequest("Please provide a valid delivery location for delivery orders.");
            }

            // Map DTO to Order model
            var order = _mapper.Map<Order>(dto);
            order.OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            order.Status = "Pending";
            order.TotalAmount = 0;
            order.OrderDate = DateTime.UtcNow;
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.ScheduledTime = dto.ScheduledTime;

            // Calculate totals and update related items
            foreach (var item in order.OrderItems!)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                if (menuItem == null)
                    return BadRequest($"MenuItem ID {item.MenuItemId} not found.");

                item.UnitPrice = menuItem.Price;
                item.SubTotal = item.UnitPrice * item.Quantity;
                order.TotalAmount += item.SubTotal;
                menuItem.OrderCount += item.Quantity;
                item.CreatedAt = DateTime.UtcNow;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return created order with populated data
            var result = await _context.Orders
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, _mapper.Map<OrderDTO>(result));
        }

        // ---------------- GET ACTIVE ORDERS ----------------
        // GET: api/orders/active
        // Role: Admin, Staff
        [Authorize(Roles = "Staff, Admin")]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetActiveOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "Pending" && o.PaymentStatus == "Succeeded")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<List<OrderDTO>>(orders));
        }

        // ---------------- MARK ORDER AS COMPLETED ----------------
        // PUT: api/orders/{id}/complete
        // Role: Admin, Staff
        [Authorize(Roles = "Staff, Admin")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkOrderAsCompleted(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.Status != "Pending" || order.PaymentStatus != "Succeeded")
                return BadRequest("Only paid and pending orders can be marked as completed.");

            order.Status = "Completed";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Order marked as completed." });
        }
    }
}
