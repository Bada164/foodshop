using foodshop.Models;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{
    public class ProductRepository : IProductRepository
    {
        DataContext _entityFramework;

        private readonly IConfiguration _config;

        public ProductRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
            _config = config;
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public IEnumerable<Product> GetAllActiveProducts()
        {
            return _entityFramework.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToList();
        }


        public IEnumerable<Product> GetAllProductsAdmin()
        {
            return _entityFramework.Products
                .Include(p => p.Category)
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            return _entityFramework.Products.Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(Product product)
        {
            _entityFramework.Products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _entityFramework.Products.Update(product);
        }

        public void DeleteProduct(Product product)
        {
            _entityFramework.Products.Remove(product);
        }

    }
}