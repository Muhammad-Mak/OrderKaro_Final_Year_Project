namespace FYP_Backend.DTOs.MenuItem
{
    public class UpdateMenuItemDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
    }
}
