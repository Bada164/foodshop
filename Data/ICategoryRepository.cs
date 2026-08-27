using foodshop.DTOs;
using foodshop.Models;

namespace foodshop.Data
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        Category CreateCategory(CategoryCreateUpdateDto dto);
        bool UpdateCategory(int id, CategoryCreateUpdateDto dto);
        bool DeleteCategory(int id);
    }
}