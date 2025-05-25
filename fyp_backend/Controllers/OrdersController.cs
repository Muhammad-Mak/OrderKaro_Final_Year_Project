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
    [Route("api/[controller]")] // Route becomes: api/orders
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context; // EF Core DB context
        private readonly IMapper _mapper;       // AutoMapper for mapping DTOs <-> models

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/orders/user/5
        // Returns all orders for a specific user
        [Authorize(Roles = "Customer, Staff, Admin")] 
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersForUser(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)               // Include user details
                .Include(o => o.OrderItems!)               // Include order items
                    .ThenInclude(oi => oi.MenuItem)        // Include associated menu items
                .OrderByDescending(o => o.OrderDate)       // Latest orders first
                .ToListAsync();

            return Ok(_mapper.Map<List<OrderDTO>>(orders)); // Convert to DTOs and return
        }

        // GET: api/orders
        // Returns all orders in the system
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)               // Include user details
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(_mapper.Map<List<OrderDTO>>(orders));
        }


        // GET: api/orders/5
        // Returns a specific order by its ID
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound(); // Return 404 if order doesn't exist

            return Ok(_mapper.Map<OrderDTO>(order)); // Return order as DTO
        }

        // POST: api/orders
        // Creates a new order from a CreateOrderDTO
        [Authorize(Roles = "Customer, Staff, Admin")]
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            // If no scheduled time is provided, set it to now
            if (dto.ScheduledTime == null)
            {
                dto.ScheduledTime = DateTime.UtcNow;
            }
            // If scheduled time is in the past (more than 1 min), reject the request
            else if (dto.ScheduledTime < DateTime.UtcNow.AddMinutes(-1))
            {
                return BadRequest("Scheduled time must be now or in the future.");
            }

            // If order is for delivery, a location must be provided
            if (dto.OrderType == "Delivery" && string.IsNullOrWhiteSpace(dto.DeliveryLocation))
            {
                return BadRequest("Please provide a valid delivery location for delivery orders.");
            }

            // Map the order DTO to the Order model
            var order = _mapper.Map<Order>(dto);
            order.OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); // Generate short order number
            order.Status = "Pending";              // Default order status
            order.TotalAmount = 0;                 // Will be calculated below
            order.OrderDate = DateTime.UtcNow;
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.ScheduledTime = dto.ScheduledTime;

            // Loop through each item in the order to calculate prices and totals
            foreach (var item in order.OrderItems!)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                if (menuItem == null)
                    return BadRequest($"MenuItem ID {item.MenuItemId} not found."); // Return error if item doesn't exist

                item.UnitPrice = menuItem.Price;                  // Get price from menu item
                item.SubTotal = item.UnitPrice * item.Quantity;   // Calculate subtotal for item
                order.TotalAmount += item.SubTotal;               // Add to total amount
                menuItem.OrderCount += item.Quantity;           // Update menu item order count
                item.CreatedAt = DateTime.UtcNow;                 // Timestamp
            }

            // Save the new order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reload the order from DB with related data for response
            var result = await _context.Orders
                .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            // Return 201 Created with route to GetOrder
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, _mapper.Map<OrderDTO>(result));
        }

        [Authorize(Roles = "Staff, Admin")]
        // GET: api/orders/active
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
        // PUT: api/orders/{id}/complete
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
