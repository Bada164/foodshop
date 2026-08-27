namespace foodshop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;


        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
        public string? Allergens { get; set; }
        public bool IsActive { get; set; }

        public Product()
        {
            if (Name == null)
            {
                Name = "";
            }
            if (Description == null)
            {
                Description = "";
            }
            if (ImageUrl == null)
            {
                ImageUrl = "";
            }
            if (Allergens == null)
            {
                Allergens = "";
            }
        }
    }
}