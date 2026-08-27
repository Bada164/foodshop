using foodshop.DTOs;
using foodshop.Models;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{
    public class CategoryRepository : ICategoryRepository
    {
        DataContext _entityFramework;



        public CategoryRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);

        }



        public IEnumerable<Category> GetAllCategories()
        {
            return _entityFramework.Categories.ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _entityFramework.Categories.FirstOrDefault(c => c.Id == id);
        }

        public Category CreateCategory(CategoryCreateUpdateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _entityFramework.Categories.Add(category);
            _entityFramework.SaveChanges();
            return category;
        }

        public bool UpdateCategory(int id, CategoryCreateUpdateDto dto)
        {
            var category = _entityFramework.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return false;

            category.Name = dto.Name;
            category.Description = dto.Description;

            return _entityFramework.SaveChanges() > 0;
        }

        public bool DeleteCategory(int id)
        {
            var category = _entityFramework.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return false;

            // Biztonsági ellenőrzés: Van-e olyan termék, ami ehhez a kategóriához tartozik?
            var hasProducts = _entityFramework.Products.Any(p => p.CategoryId == id);
            if (hasProducts)
            {
                throw new Exception("Ez a kategória nem törölhető, mert még vannak benne termékek!");
            }

            _entityFramework.Categories.Remove(category);
            return _entityFramework.SaveChanges() > 0;
        }
    }
}