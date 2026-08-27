using foodshop.Models;

namespace foodshop.Data
{
    public interface IProductRepository
    {

        public bool SaveChanges();
        public IEnumerable<Product> GetAllActiveProducts();

        public IEnumerable<Product> GetAllProductsAdmin();
        Product? GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
    }
}