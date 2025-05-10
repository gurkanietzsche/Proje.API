using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public ProductController(
            ProductRepository productRepository,
            CategoryRepository categoryRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        // GET: api/Product
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetProductsWithCategoryAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);
                return Ok(productDtos);
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Ürünler getirilirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductWithCategoryAsync(id);

            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }

        // GET: api/Product/Category/5
        [HttpGet("Category/{categoryId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategory(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                _result.Status = false;
                _result.Message = "Kategori bulunamadı";
                return NotFound(_result);
            }

            var products = await _productRepository.FindAsync(p => p.CategoryId == categoryId);
            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productDtos);
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDTO>> CreateProduct(ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz ürün bilgileri";
                return BadRequest(_result);
            }

            try
            {
                var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
                if (category == null)
                {
                    _result.Status = false;
                    _result.Message = "Kategori bulunamadı";
                    return BadRequest(_result);
                }

                var product = _mapper.Map<Product>(productDto);
                // Zorunlu alanları manuel olarak set edin
                product.Created = DateTime.Now;
                product.Updated = DateTime.Now;
                product.IsActive = true;

                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Ürün başarıyla eklendi";
                _result.Data = product.Id;

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, _result);
            }
            catch (Exception ex)
            {
                _result.Status = false;
                // Inner exception'ı da yakalayın
                _result.Message = $"Ürün eklenirken bir hata oluştu: {ex.Message}";
                if (ex.InnerException != null)
                {
                    _result.Message += $" Inner Exception: {ex.InnerException.Message}";
                }
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDTO>> UpdateProduct(int id, ProductDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz ürün bilgileri";
                return BadRequest(_result);
            }

            if (id != productDto.Id)
            {
                _result.Status = false;
                _result.Message = "ID uyuşmazlığı";
                return BadRequest(_result);
            }

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            try
            {
                var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
                if (category == null)
                {
                    _result.Status = false;
                    _result.Message = "Kategori bulunamadı";
                    return BadRequest(_result);
                }

                _mapper.Map(productDto, product);

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Ürün başarıyla güncellendi";

                return _result;
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Ürün güncellenirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDTO>> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            try
            {
                await _productRepository.DeleteAsync(id);
                await _productRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Ürün başarıyla silindi";

                return _result;
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Ürün silinirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }

        // PUT: api/Product/5/stock
        [HttpPut("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResultDTO>> UpdateStock(int id, int stock)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            try
            {
                product.Stock = stock;
                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                _result.Status = true;
                _result.Message = "Ürün stoğu başarıyla güncellendi";

                return _result;
            }
            catch (System.Exception ex)
            {
                _result.Status = false;
                _result.Message = "Ürün stoğu güncellenirken bir hata oluştu: " + ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _result);
            }
        }
    }
}