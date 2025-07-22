using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Backend.Models;
using FYP_Backend.DTOs.User;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using FYP_Backend.Context;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: api/auth
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;         // EF Core DB context for Users table
        private readonly IMapper _mapper;               // AutoMapper for DTO ↔ Entity conversion
        private readonly IConfiguration _configuration; // Used to access JWT settings

        public AuthController(AppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        // ------------------ REGISTER ------------------
        // POST: api/auth/register
        // Registers a new user with role "Customer" and hashed password
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            // Check for duplicate email
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email is already in use.");

            // Map DTO to User entity and set additional fields
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = HashPassword(dto.Password); // Secure password hash
            user.Role = "Customer";                         // Default user role
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful." });
        }

        // ------------------ LOGIN ------------------
        // POST: api/auth/login
        // Authenticates user and returns a JWT token + safe user data
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Validate user existence and password
            if (user == null || user.PasswordHash != HashPassword(dto.Password))
                return Unauthorized("Invalid email or password.");

            // Generate JWT and return user DTO
            var token = GenerateJwtToken(user);
            var userDto = _mapper.Map<UserDTO>(user);

            return Ok(new { token, user = userDto });
        }

        // ------------------ JWT GENERATION ------------------
        // Helper method to build a JWT token with claims
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ------------------ PASSWORD HASHING ------------------
        // Hashes a plain text password using SHA-256
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
