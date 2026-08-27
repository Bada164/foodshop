namespace foodshop.DTOs
{
    // Ezt küldjük vissza a Frontendnek (pl. a menü kirajzolásához)
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}