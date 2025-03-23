using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Repositories;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bu satırı ekleyin veya düzeltin
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ResultDTO _result;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _result = new ResultDTO();
        }

        [HttpGet]
        [AllowAnonymous] // İsteğe bağlı: Listeleme için yetki gerektirmeyin
        public async Task<ActionResult<ResultDTO>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            _result.Status = true;
            _result.Message = "Categories retrieved successfully";
            _result.Data = categories;
            return Ok(_result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // İsteğe bağlı: Detay görüntüleme için yetki gerektirmeyin
        public async Task<ActionResult<ResultDTO>> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _result.Status = false;
                _result.Message = "Category not found";
                return NotFound(_result);
            }
            _result.Status = true;
            _result.Message = "Category retrieved successfully";
            _result.Data = category;
            return Ok(_result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar ekleme yapabilir
        public async Task<ActionResult<ResultDTO>> CreateCategory(CategoryDTO categoryDto)
        {
            var category = await _categoryRepository.CreateCategoryAsync(categoryDto);
            _result.Status = true;
            _result.Message = "Category created successfully";
            _result.Data = category;
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, _result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar güncelleme yapabilir
        public async Task<ActionResult<ResultDTO>> UpdateCategory(int id, CategoryDTO categoryDto)
        {
            if (!await _categoryRepository.CategoryExistsAsync(id))
            {
                _result.Status = false;
                _result.Message = "Category not found";
                return NotFound(_result);
            }

            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(id, categoryDto);
            _result.Status = true;
            _result.Message = "Category updated successfully";
            _result.Data = updatedCategory;
            return Ok(_result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar silme yapabilir
        public async Task<ActionResult<ResultDTO>> DeleteCategory(int id)
        {
            if (!await _categoryRepository.CategoryExistsAsync(id))
            {
                _result.Status = false;
                _result.Message = "Category not found";
                return NotFound(_result);
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            _result.Status = true;
            _result.Message = "Category deleted successfully";
            return Ok(_result);
        }
    }
}