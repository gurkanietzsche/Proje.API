using Microsoft.EntityFrameworkCore;
using Proje.API.Data;
using Proje.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mevcut metodlarınız burada kalacak
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Yeni eklenen kategori hiyerarşisi metodları
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