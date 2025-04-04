namespace FYP_Backend.DTOs.MenuItem
{
    public class MenuItemDTO
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
