using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly ProductReviewRepository _reviewRepository;
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ResultDTO _result;

        public ProductReviewController(
            ProductReviewRepository reviewRepository,
            ProductRepository productRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _result = new ResultDTO();
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductReviewDTO>>> GetProductReviews(int productId)
        {
            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
            var reviewDtos = _mapper.Map<IEnumerable<ProductReviewDTO>>(reviews);
            return Ok(reviewDtos);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateReview(CreateProductReviewDTO reviewDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz yorum bilgileri";
                return BadRequest(_result);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var product = await _productRepository.GetByIdAsync(reviewDto.ProductId);
            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            var review = new ProductReview
            {
                ProductId = reviewDto.ProductId,
                UserId = userId,
                ReviewText = reviewDto.ReviewText,
                Rating = reviewDto.Rating,
                IsApproved = false // Varsayılan olarak onay bekliyor
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Yorum başarıyla eklendi, onay bekliyor";
            return Ok(_result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{reviewId}/approve")]
        public async Task<ActionResult<ResultDTO>> ApproveReview(int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                _result.Status = false;
                _result.Message = "Yorum bulunamadı";
                return NotFound(_result);
            }

            review.IsApproved = true;
            await _reviewRepository.UpdateAsync(review);
            await _reviewRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Yorum başarıyla onaylandı";
            return Ok(_result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<ResultDTO>> DeleteReview(int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                _result.Status = false;
                _result.Message = "Yorum bulunamadı";
                return NotFound(_result);
            }

            await _reviewRepository.DeleteAsync(reviewId);
            await _reviewRepository.SaveChangesAsync();

            _result.Status = true;
            _result.Message = "Yorum başarıyla silindi";
            return Ok(_result);
        }
    }
}