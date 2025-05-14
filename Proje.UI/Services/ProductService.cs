using Proje.UI.Models.DTOs;

namespace Proje.UI.Services
{
    public class ProductService
    {
        private readonly ApiService _apiService;

        public ProductService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return await _apiService.GetAsync<IEnumerable<ProductDTO>>("api/Product");
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            return await _apiService.GetAsync<ProductDTO>($"api/Product/{id}");
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _apiService.GetAsync<IEnumerable<ProductDTO>>($"api/Product/Category/{categoryId}");
        }

        public async Task<ResultDTO> CreateProductAsync(ProductDTO productDto)
        {
            return await _apiService.PostAsync<ResultDTO>("api/Product", productDto);
        }

        public async Task<ResultDTO> UpdateProductAsync(ProductDTO productDto)
        {
            return await _apiService.PutAsync<ResultDTO>($"api/Product/{productDto.Id}", productDto);
        }

        public async Task<ResultDTO> DeleteProductAsync(int id)
        {
            return await _apiService.DeleteAsync<ResultDTO>($"api/Product/{id}");
        }

        public async Task<ResultDTO> UpdateStockAsync(int id, int stock)
        {
            return await _apiService.PutAsync<ResultDTO>($"api/Product/{id}/stock?stock={stock}", null);
        }
    }
}