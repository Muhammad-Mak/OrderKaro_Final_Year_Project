using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FYP_Backend.DTOs.User;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route becomes: api/users
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context; // Database context
        private readonly IMapper _mapper;       // AutoMapper for DTO conversion

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/users
        // Retrieves all users in the system
        [Authorize(Roles = "Admin")] // Only accessible to Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(_mapper.Map<List<UserDTO>>(users)); // Return mapped list of user DTOs
        }

        // GET: api/users/5
        // Retrieves a single user by ID
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(); // Return 404 if user not found

            return Ok(_mapper.Map<UserDTO>(user));
        }

        // PUT: api/users/5
        // Updates an existing user with new data
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _mapper.Map(dto, user); // Apply DTO fields to the user entity
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(); // Save changes to DB
            return NoContent(); // Return 204 No Content
        }

        // DELETE: api/users/5
        // Deletes a user by ID
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/users/topup
        // Top up a user's RFID balance using their Student ID
        [Authorize(Roles = "Admin, Staff")] // Both Admin and Staff can access this
        [HttpPost("topup")]
        public async Task<IActionResult> TopUpBalance(string studentId, decimal amount)
        {
            // Find user by StudentId (unique RFID identifier)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
            if (user == null) return NotFound("Student not found.");

            user.Balance += amount;               // Add the top-up amount to user's balance
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();    // Save updated balance to the database

            return Ok(new { user.Balance });      // Return new balance
        }
    }
}
