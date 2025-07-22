using AutoMapper;
using FYP_Backend.Context;
using FYP_Backend.DTOs.MenuItem;
using FYP_Backend.Models;
using Microsoft.AspNetCore.Authorization;
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

        // ---------------- GET ALL ----------------
        // GET: api/menuitems
        // Returns all menu items including category info
        [Authorize(Roles = "Admin, Staff, Customer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenuItems()
        {
            var items = await _context.MenuItems
                .Include(m => m.Category) // Eager-load related category
                .ToListAsync();

            var dtoList = _mapper.Map<List<MenuItemDTO>>(items);
            return Ok(dtoList);
        }

        // ---------------- GET BY ID ----------------
        // GET: api/menuitems/{id}
        // Fetches a specific menu item with category info
        [Authorize(Roles = "Admin, Staff, Customer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDTO>> GetMenuItem(int id)
        {
            var item = await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.MenuItemId == id);

            if (item == null)
                return NotFound();

            var dto = _mapper.Map<MenuItemDTO>(item);
            return Ok(dto);
        }

        // ---------------- CREATE ----------------
        // POST: api/menuitems
        // Creates a new menu item
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        public async Task<ActionResult<MenuItemDTO>> CreateMenuItem([FromBody] CreateMenuItemDTO dto)
        {
            var menuItem = _mapper.Map<MenuItem>(dto);
            menuItem.CreatedAt = DateTime.UtcNow;
            menuItem.UpdatedAt = DateTime.UtcNow;

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<MenuItemDTO>(menuItem);

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.MenuItemId }, resultDto);
        }

        // ---------------- UPDATE ----------------
        // PUT: api/menuitems/{id}
        // Updates an existing menu item by ID
        [Authorize(Roles = "Admin, Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDTO dto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _mapper.Map(dto, menuItem);
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent(); // 204 - success, no content returned
        }

        // ---------------- DELETE ----------------
        // DELETE: api/menuitems/{id}
        // Deletes a menu item by its ID
        [Authorize(Roles = "Admin, Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 - deleted successfully
        }

        // ---------------- GET BY CATEGORY ----------------
        // GET: api/menuitems/by-category/{categoryId}
        // Returns all available menu items under a specific category
        [Authorize(Roles = "Admin, Staff, Customer")]
        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenuItemsByCategory(int categoryId)
        {
            var items = await _context.MenuItems
                .Include(m => m.Category)
                .Where(m => m.CategoryId == categoryId && m.IsAvailable)
                .ToListAsync();

            return Ok(_mapper.Map<List<MenuItemDTO>>(items));
        }
    }
}
