using AutoMapper;
using FYP_Backend.Context;
using FYP_Backend.DTOs.MenuItem;
using FYP_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FYP_Backend.Controllers
{
    // Marks this class as an API controller and sets the route to: api/menuitems
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly AppDbContext _context;  // EF Core DB context
        private readonly IMapper _mapper;        // AutoMapper for DTOs <-> Models

        public MenuItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/menuitems
        // Returns all menu items including their category information
        [Authorize(Roles = "Admin, Staff")] // Access restricted to Admin and Staff roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenuItems()
        {
            var items = await _context.MenuItems
                .Include(m => m.Category) // Eagerly load related category data
                .ToListAsync();

            var dtoList = _mapper.Map<List<MenuItemDTO>>(items); // Convert to DTOs
            return Ok(dtoList);
        }

        // GET: api/menuitems/5
        // Returns a specific menu item by ID
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDTO>> GetMenuItem(int id)
        {
            var item = await _context.MenuItems
                .Include(m => m.Category) // Include category info
                .FirstOrDefaultAsync(m => m.MenuItemId == id);

            if (item == null)
                return NotFound(); // Return 404 if not found

            var dto = _mapper.Map<MenuItemDTO>(item);
            return Ok(dto);
        }

        // POST: api/menuitems
        // Creates a new menu item
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        public async Task<ActionResult<MenuItemDTO>> CreateMenuItem([FromBody] CreateMenuItemDTO dto)
        {
            var menuItem = _mapper.Map<MenuItem>(dto); // Map incoming data to MenuItem model
            menuItem.CreatedAt = DateTime.UtcNow;
            menuItem.UpdatedAt = DateTime.UtcNow;

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<MenuItemDTO>(menuItem);
            // Return 201 Created with reference to GetMenuItem route
            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.MenuItemId }, resultDto);
        }

        // PUT: api/menuitems/5
        // Updates an existing menu item
        [Authorize(Roles = "Admin, Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDTO dto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _mapper.Map(dto, menuItem); // Update fields from DTO
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent(); // Return 204
        }

        // DELETE: api/menuitems/5
        // Deletes a menu item by ID
        [Authorize(Roles = "Admin, Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent(); // Return 204
        }
    }
}
