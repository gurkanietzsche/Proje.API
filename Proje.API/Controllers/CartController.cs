using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Proje.API.DTOs;
using Proje.API.Models;
using Proje.API.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proje.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartRepository _cartRepository;
        private readonly ProductRepository _productRepository;
        private readonly ResultDTO _result;

        public CartController(CartRepository cartRepository, ProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _result = new ResultDTO();
        }

        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return Ok(new CartDTO { UserId = userId, CartItems = new List<CartItemDTO>() });
            }

            var cartDto = new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CartItems = cart.CartItems?.Select(item => new CartItemDTO
                {
                    Id = item.Id,
                    CartId = item.CartId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name,
                    ProductImage = item.Product?.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product?.Price ?? 0
                }).ToList() ?? new List<CartItemDTO>()
            };

            return Ok(cartDto);
        }

        [HttpPost("items")]
        public async Task<ActionResult<ResultDTO>> AddToCart(AddToCartDTO addToCartDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz veri gönderildi";
                return BadRequest(_result);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            // Ürünü kontrol et
            var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
            if (product == null)
            {
                _result.Status = false;
                _result.Message = "Ürün bulunamadı";
                return NotFound(_result);
            }

            // Stok kontrolü
            if (product.Stock < addToCartDto.Quantity)
            {
                _result.Status = false;
                _result.Message = "Yeterli stok yok";
                return BadRequest(_result);
            }

            // Kullanıcının sepetini al veya yeni oluştur
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                // Yeni sepet oluştur
                cart = new Cart { UserId = userId };
                // CreateCartAsync metodu olmadığı için doğrudan AddAsync kullanıyoruz
                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }

            // Sepete ürün ekle
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity
            };

            await _cartRepository.AddItemToCartAsync(cartItem);

            _result.Status = true;
            _result.Message = "Ürün sepete eklendi";
            return _result;
        }

        [HttpPut("items/{itemId}")]
        public async Task<ActionResult<ResultDTO>> UpdateCartItem(int itemId, UpdateCartItemDTO updateCartItemDto)
        {
            if (!ModelState.IsValid)
            {
                _result.Status = false;
                _result.Message = "Geçersiz veri gönderildi";
                return BadRequest(_result);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _result.Status = false;
                _result.Message = "Sepet bulunamadı";
                return NotFound(_result);
            }

            var cartItem = cart.CartItems?.FirstOrDefault(i => i.Id == itemId);
            if (cartItem == null)
            {
                _result.Status = false;
                _result.Message = "Sepet öğesi bulunamadı";
                return NotFound(_result);
            }

            // Stok kontrolü
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null || product.Stock < updateCartItemDto.Quantity)
            {
                _result.Status = false;
                _result.Message = "Yeterli stok yok";
                return BadRequest(_result);
            }

            cartItem.Quantity = updateCartItemDto.Quantity;
            await _cartRepository.UpdateCartItemAsync(cartItem);

            _result.Status = true;
            _result.Message = "Sepet öğesi güncellendi";
            return _result;
        }

        [HttpDelete("items/{itemId}")]
        public async Task<ActionResult<ResultDTO>> RemoveCartItem(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _result.Status = false;
                _result.Message = "Sepet bulunamadı";
                return NotFound(_result);
            }

            var cartItem = cart.CartItems?.FirstOrDefault(i => i.Id == itemId);
            if (cartItem == null)
            {
                _result.Status = false;
                _result.Message = "Sepet öğesi bulunamadı";
                return NotFound(_result);
            }

            await _cartRepository.RemoveCartItemAsync(itemId);

            _result.Status = true;
            _result.Message = "Ürün sepetten kaldırıldı";
            return _result;
        }

        [HttpDelete]
        public async Task<ActionResult<ResultDTO>> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _result.Status = false;
                _result.Message = "Kullanıcı bilgisi bulunamadı";
                return BadRequest(_result);
            }

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _result.Status = false;
                _result.Message = "Sepet bulunamadı";
                return NotFound(_result);
            }

            await _cartRepository.ClearCartAsync(cart.Id);

            _result.Status = true;
            _result.Message = "Sepet temizlendi";
            return _result;
        }
    }
}