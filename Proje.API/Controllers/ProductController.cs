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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public ProductController(
            IProductRepository productRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _productRepository.GetProductsWithCategoryAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);
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

            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateProduct(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Ürün başarıyla eklendi";
            _result.Data = product.Id;

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, _result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultDTO>> UpdateProduct(int id, ProductDTO productDto)
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

            _mapper.Map(productDto, product);

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Ürün başarıyla güncellendi";

            return _result;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultDTO>> DeleteProduct(int id)
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