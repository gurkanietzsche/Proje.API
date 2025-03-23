using Proje.API.Data;
using Proje.API.Models;

namespace Proje.API.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

    public interface ICategoryRepository : IRepository<Category>
    {
    }
}