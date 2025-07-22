using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FYP_Backend.DTOs.User;
using FYP_Backend.Context;
using Microsoft.AspNetCore.Authorization;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route: api/users
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ------------------ GET ALL USERS ------------------
        // GET: api/users
        // Role: Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(_mapper.Map<List<UserDTO>>(users));
        }

        // ------------------ GET USER BY ID ------------------
        // GET: api/users/{id}
        // Role: Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(_mapper.Map<UserDTO>(user));
        }

        // ------------------ UPDATE USER ------------------
        // PUT: api/users/{id}
        // Role: Admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _mapper.Map(dto, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent(); // 204 - success without return data
        }

        // ------------------ DELETE USER ------------------
        // DELETE: api/users/{id}
        // Role: Admin only
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

        // ------------------ TOP-UP BALANCE ------------------
        // POST: api/users/topup?studentId=...&amount=...
        // Roles: Admin, Staff
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost("topup")]
        public async Task<IActionResult> TopUpBalance(string studentId, decimal amount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
            if (user == null) return NotFound("Student not found.");

            user.Balance += amount;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { user.Balance });
        }

        // ------------------ GET USER COUNT ------------------
        // GET: api/users/count
        // Role: Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet("count")]
        public async Task<IActionResult> GetUserCount()
        {
            var totalUsers = await _context.Users.CountAsync();
            return Ok(new { totalUsers });
        }
    }
}
