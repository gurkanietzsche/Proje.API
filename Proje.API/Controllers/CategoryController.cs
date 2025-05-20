using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public CategoryController(
            CategoryRepository categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        // Mevcut endpointler burada kalacak
        [HttpGet]
        [Authorize] // Sadece giriş yapmış kullanıcılar için
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
                return Ok(categoryDtos);
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Kategoriler getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        [HttpGet("{id}")]
        [Authorize] // Sadece giriş yapmış kullanıcılar için
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                _result.Status = false;
                _result.Message = "Kategori bulunamadı";
                return NotFound(_result);
            }

            var categoryDto = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateCategory(CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz kategori bilgileri";
                return BadRequest(_result);
            }

            try
            {
                // Yeni bir model oluştur
                var category = new Category
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    ParentCategoryId = categoryDto.ParentCategoryId,
                    IsActive = categoryDto.IsActive,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };

                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Kategori başarıyla eklendi";
                _result.Data = category.Id;

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, _result);
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Kategori eklenirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultDTO>> UpdateCategory(int id, CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz kategori bilgileri";
                return BadRequest(_result);
            }

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

            try
            {
                // Manuel olarak category özelliklerini güncelleyelim
                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;
                category.ParentCategoryId = categoryDto.ParentCategoryId;
                category.IsActive = categoryDto.IsActive;
                category.Updated = DateTime.Now;

                await _categoryRepository.UpdateAsync(category);
                await _categoryRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Kategori başarıyla güncellendi";

                return _result;
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Kategori güncellenirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
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

            try
            {
                await _categoryRepository.DeleteAsync(id);
                await _categoryRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Kategori başarıyla silindi";

                return _result;
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Kategori silinirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        // Kategori hiyerarşisi endpointleri
        [HttpGet("main")]
        [Authorize] // Sadece giriş yapmış kullanıcılar için
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetMainCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetMainCategoriesAsync();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
                return Ok(categoryDtos);
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Ana kategoriler getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        [HttpGet("children/{parentId}")]
        [Authorize] // Sadece giriş yapmış kullanıcılar için
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetChildCategories(int parentId)
        {
            try
            {
                var categories = await _categoryRepository.GetChildCategoriesAsync(parentId);
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
                return Ok(categoryDtos);
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Alt kategoriler getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        [HttpGet("hierarchy/{id}")]
        [Authorize] // Sadece giriş yapmış kullanıcılar için
        public async Task<ActionResult<CategoryDTO>> GetCategoryHierarchy(int id)
        {
            try
            {
                var category = await _categoryRepository.GetFullCategoryHierarchyAsync(id);

                if (category == null)
                {
                    _result.Status = false;
                    _result.Message = "Kategori bulunamadı";
                    return NotFound(_result);
                }

                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return Ok(categoryDto);
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Kategori hiyerarşisi getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }
    }
}