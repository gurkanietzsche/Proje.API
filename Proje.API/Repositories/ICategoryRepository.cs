using Proje.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);  // Bu metot tanımlanmış olmalı
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}