using Proje.UI.Models.DTOs;

namespace Proje.UI.Services
{
    public class CartService
    {
        private readonly ApiService _apiService;

        public CartService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<CartDTO> GetCartAsync()
        {
            return await _apiService.GetAsync<CartDTO>("api/Cart");
        }

        public async Task<ResultDTO> AddToCartAsync(int productId, int quantity)
        {
            var addToCartDto = new { ProductId = productId, Quantity = quantity };
            return await _apiService.PostAsync<ResultDTO>("api/Cart/items", addToCartDto);
        }

        public async Task<ResultDTO> UpdateCartItemAsync(int itemId, int quantity)
        {
            var updateCartItemDto = new { Quantity = quantity };
            return await _apiService.PutAsync<ResultDTO>($"api/Cart/items/{itemId}", updateCartItemDto);
        }

        public async Task<ResultDTO> RemoveCartItemAsync(int itemId)
        {
            return await _apiService.DeleteAsync<ResultDTO>($"api/Cart/items/{itemId}");
        }

        public async Task<ResultDTO> ClearCartAsync()
        {
            return await _apiService.DeleteAsync<ResultDTO>("api/Cart");
        }
    }
}