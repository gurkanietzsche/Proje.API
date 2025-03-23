using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly Result _result;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _result = new Result();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _productRepository.GetProductsWithCategoryAsync();

            var productDtos = products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                IsActive = p.IsActive
            }).ToList();

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductWithCategoryAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                IsActive = product.IsActive
            };

            return Ok(productDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result>> CreateProduct(ProductDTO productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId,
                IsActive = productDto.IsActive
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Ürün başarıyla eklendi";
            _result.Data = product.Id;

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, _result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> UpdateProduct(int id, ProductDTO productDto)
        {
            if (id != productDto.Id)
            {
                _result.Status = false;
                _result.Message = "Id uyuşmazlığı";
                return BadRequest(_result);
            }

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Stock = productDto.Stock;
            product.CategoryId = productDto.CategoryId;
            product.IsActive = productDto.IsActive;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Ürün başarıyla güncellendi";

            return _result;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Ürün başarıyla silindi";

            return _result;
        }
    }
}