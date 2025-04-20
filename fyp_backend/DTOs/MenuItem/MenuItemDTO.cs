namespace FYP_Backend.DTOs.MenuItem
{
    public class MenuItemDTO
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = null!;
    }
}
