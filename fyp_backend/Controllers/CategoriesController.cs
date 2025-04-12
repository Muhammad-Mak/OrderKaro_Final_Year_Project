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
        private readonly AppDbContext _context; // Database context
        private readonly IMapper _mapper;       // AutoMapper for DTO to model mapping

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/categories
        // Retrieves all categories from the database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_mapper.Map<List<CategoryDTO>>(categories)); // Maps and returns as DTOs
        }

        // GET: api/categories/5
        // Retrieves a single category by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(); // Return 404 if not found
            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        // POST: api/categories
        // Creates a new category using data from CreateCategoryDTO
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CreateCategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto); // Convert DTO to entity
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CategoryDTO>(category);
            // Return 201 with route to newly created resource
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, result);
        }

        // PUT: api/categories/5
        // Updates an existing category with new data
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _mapper.Map(dto, category); // Apply changes from DTO
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent(); // Return 204 - successful with no content
        }

        // DELETE: api/categories/5
        // Deletes the category with the given ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent(); // Return 204
        }
    }
}
