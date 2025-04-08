namespace FYP_Backend.DTOs.User
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!; // Only for admin control
    }
}
