using Proje.API.Models;

namespace Proje.API.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
        Task<Product> GetProductWithCategoryAsync(int id);
    }
}