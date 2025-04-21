using Microsoft.EntityFrameworkCore;
using Proje.API.Data;
using Proje.API.Models;

namespace Proje.API.Repositories
{
    public class ProductReviewRepository : GenericRepository<ProductReview>
    {
        public ProductReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.ProductReviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetPendingReviewsAsync()
        {
            return await _context.ProductReviews
                .Where(r => !r.IsApproved)
                .ToListAsync();
        }
    }
}