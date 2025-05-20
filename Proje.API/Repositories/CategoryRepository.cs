using Microsoft.EntityFrameworkCore;
using Proje.API.Data;
using Proje.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public class CategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            // Eğer oluşturma zamanı ayarlanmamışsa otomatik ayarla
            if (category.Created == default)
            {
                category.Created = System.DateTime.Now;
            }

            // Güncelleme zamanını her zaman şimdiki zaman olarak ayarla
            category.Updated = System.DateTime.Now;

            await _context.Categories.AddAsync(category);
        }

        public Task UpdateAsync(Category category)
        {
            // Güncelleme zamanını her zaman şimdiki zaman olarak ayarla
            category.Updated = System.DateTime.Now;

            _context.Categories.Update(category);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Kategori hiyerarşisi metodları
        public async Task<IEnumerable<Category>> GetMainCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryWithChildrenAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> GetFullCategoryHierarchyAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.ChildCategories)
                .ThenInclude(c => c.ChildCategories)  // İki seviye alt kategoriye kadar yükle
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}