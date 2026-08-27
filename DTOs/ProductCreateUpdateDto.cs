namespace foodshop.DTOs
{
    public class ProductCreateUpdateDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Allergens { get; set; }
        public bool IsActive { get; set; }
    }
}