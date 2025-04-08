using Microsoft.EntityFrameworkCore;
using Proje.API.Data;
using Proje.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public class CartRepository : GenericRepository<Cart>
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Kullanıcının sepeti yoksa yeni bir sepet oluştur
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart> GetCartWithItemsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<CartItem> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task AddItemToCartAsync(CartItem item)
        {
            var existingItem = await GetCartItemAsync(item.CartId, item.ProductId);

            if (existingItem != null)
            {
                // Ürün zaten sepette varsa miktarını güncelle
                existingItem.Quantity += item.Quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                // Ürün sepette yoksa yeni ekle
                await _context.CartItems.AddAsync(item);
            }

            // Sepet güncelleme tarihini güncelle
            var cart = await _context.Carts.FindAsync(item.CartId);
            if (cart != null)
            {
                cart.ModifiedDate = DateTime.Now;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);

            // Sepet güncelleme tarihini güncelle
            var cart = await _context.Carts.FindAsync(item.CartId);
            if (cart != null)
            {
                cart.ModifiedDate = DateTime.Now;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);

                // Sepet güncelleme tarihini güncelle
                var cart = await _context.Carts.FindAsync(cartItem.CartId);
                if (cart != null)
                {
                    cart.ModifiedDate = DateTime.Now;
                    _context.Carts.Update(cart);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            var cartItems = _context.CartItems.Where(ci => ci.CartId == cartId);
            _context.CartItems.RemoveRange(cartItems);

            // Sepet güncelleme tarihini güncelle
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                cart.ModifiedDate = DateTime.Now;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();
        }
    }
}