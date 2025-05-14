using Proje.UI.Models.DTOs;

namespace Proje.UI.Services
{
    public class CategoryService
    {
        private readonly ApiService _apiService;

        public CategoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            return await _apiService.GetAsync<IEnumerable<CategoryDTO>>("api/Category");
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            return await _apiService.GetAsync<CategoryDTO>($"api/Category/{id}");
        }

        public async Task<IEnumerable<CategoryDTO>> GetMainCategoriesAsync()
        {
            return await _apiService.GetAsync<IEnumerable<CategoryDTO>>("api/Category/main");
        }

        public async Task<IEnumerable<CategoryDTO>> GetChildCategoriesAsync(int parentId)
        {
            return await _apiService.GetAsync<IEnumerable<CategoryDTO>>($"api/Category/children/{parentId}");
        }

        public async Task<ResultDTO> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            return await _apiService.PostAsync<ResultDTO>("api/Category", categoryDto);
        }

        public async Task<ResultDTO> UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            return await _apiService.PutAsync<ResultDTO>($"api/Category/{categoryDto.Id}", categoryDto);
        }

        public async Task<ResultDTO> DeleteCategoryAsync(int id)
        {
            return await _apiService.DeleteAsync<ResultDTO>($"api/Category/{id}");
        }
    }
}