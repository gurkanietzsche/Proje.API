using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.UI.Services;

namespace Proje.UI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly ProductService _productService;

        public CartController(
            CartService cartService,
            ProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var cart = await _cartService.GetCartAsync();
                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new Models.DTOs.CartDTO { CartItems = new List<Models.DTOs.CartItemDTO>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var result = await _cartService.AddToCartAsync(productId, quantity);
                if (result.Status)
                {
                    return Json(new { success = true, message = "Ürün sepete eklendi." });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int itemId, int quantity)
        {
            try
            {
                var result = await _cartService.UpdateCartItemAsync(itemId, quantity);
                if (result.Status)
                {
                    return Json(new { success = true, message = "Ürün miktarı güncellendi." });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCartItem(int itemId)
        {
            try
            {
                var result = await _cartService.RemoveCartItemAsync(itemId);
                if (result.Status)
                {
                    return Json(new { success = true, message = "Ürün sepetten kaldırıldı." });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var result = await _cartService.ClearCartAsync();
                if (result.Status)
                {
                    return Json(new { success = true, message = "Sepet temizlendi." });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}