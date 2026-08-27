namespace foodshop.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
        public string Allergens { get; set; }

        public ProductDto()
        {
            if (Name == null)
            {
                Name = "";
            }
            if (CategoryName == null)
            {
                CategoryName = "";
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