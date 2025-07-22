using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FYP_Backend.Models;
using FYP_Backend.DTOs.Category;
using FYP_Backend.Context;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route becomes: api/categories
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context; // EF Core DbContext for database access
        private readonly IMapper _mapper;       // AutoMapper to handle DTO ↔ Entity conversion

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ------------------ GET ALL ------------------
        // GET: api/categories
        // Returns a list of all categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_mapper.Map<List<CategoryDTO>>(categories)); // Return mapped DTO list
        }

        // ------------------ GET BY ID ------------------
        // GET: api/categories/{id}
        // Returns a single category by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(); // 404 if not found
            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        // ------------------ CREATE ------------------
        // POST: api/categories
        // Adds a new category
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CreateCategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto); // Convert DTO to entity
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CategoryDTO>(category);

            // Return 201 with URI to newly created resource
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, result);
        }

        // ------------------ UPDATE ------------------
        // PUT: api/categories/{id}
        // Updates an existing category with new data
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(); // 404 if not found

            _mapper.Map(dto, category); // Apply updated values from DTO
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent(); // 204 - successfully updated, no return content
        }

        // ------------------ DELETE ------------------
        // DELETE: api/categories/{id}
        // Deletes a category by its ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(); // 404 if not found

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 - successfully deleted
        }
    }
}
