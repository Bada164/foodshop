namespace foodshop.DTOs
{

    public class CategoryCreateUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}