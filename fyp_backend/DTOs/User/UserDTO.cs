namespace FYP_Backend.DTOs.User
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? StudentId { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
