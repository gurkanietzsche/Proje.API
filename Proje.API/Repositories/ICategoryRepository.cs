using Proje.API.DTOs;
using Proje.API.Models;

namespace Proje.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CategoryDTO categoryDto);
        Task<Category> UpdateCategoryAsync(int id, CategoryDTO categoryDto);
        Task DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id); // Bu metodu ekleyin
    }
}