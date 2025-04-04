namespace FYP_Backend.DTOs.Category
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
