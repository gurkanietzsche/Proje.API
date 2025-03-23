using Proje.API.Models;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> GetCartWithItemsAsync(int cartId);
        Task<CartItem> GetCartItemAsync(int cartId, int productId);
        Task AddItemToCartAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
    }
}