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
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MenuItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/menuitems
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDTO>>> GetMenuItems()
        {
            var items = await _context.MenuItems
                .Include(m => m.Category)
                .ToListAsync();

            var dtoList = _mapper.Map<List<MenuItemDTO>>(items);
            return Ok(dtoList);
        }

        // GET: api/menuitems/5
        [Authorize(Roles = "Admin, Staff")]
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

        // POST: api/menuitems
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

        // PUT: api/menuitems/5
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
            return NoContent();
        }

        // DELETE: api/menuitems/5
        [Authorize(Roles = "Admin, Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
