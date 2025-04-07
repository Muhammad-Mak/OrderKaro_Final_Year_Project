public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<MenuItem> MenuItems { get; set; } = new();
}
