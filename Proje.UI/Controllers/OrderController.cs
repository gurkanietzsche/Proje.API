using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proje.UI.Services;
using Proje.UI.Models.DTOs;

namespace Proje.UI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;

        public OrderController(
            OrderService orderService,
            CartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Models.DTOs.OrderDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderDetailsAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Checkout()
        {
            try
            {
                var cart = await _cartService.GetCartAsync();
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    TempData["ErrorMessage"] = "Sepetiniz boş, lütfen önce ürün ekleyin.";
                    return RedirectToAction("Index", "Cart");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderService.OrderCreateDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", orderDto);
            }

            try
            {
                var result = await _orderService.CreateOrderAsync(orderDto);
                if (result.Status)
                {
                    TempData["SuccessMessage"] = "Siparişiniz başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return View("Checkout", orderDto);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Checkout", orderDto);
            }
        }
    }
}