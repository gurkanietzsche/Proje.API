using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public CategoryController(
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateCategory(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Kategori başarıyla eklendi";
            _result.Data = category.Id;

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, _result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultDTO>> UpdateCategory(int id, CategoryDTO categoryDto)
        {
            if (id != categoryDto.Id)
            {
                _result.Status = false;
                _result.Message = "Id uyuşmazlığı";
                return BadRequest(_result);
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                _result.Status = false;
                _result.Message = "Kategori bulunamadı";
                return NotFound(_result);
            }

            _mapper.Map(categoryDto, category);

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Kategori başarıyla güncellendi";

            return _result;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultDTO>> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                _result.Status = false;
                _result.Message = "Kategori bulunamadı";
                return NotFound(_result);
            }

            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Kategori başarıyla silindi";

            return _result;
        }
    }
}