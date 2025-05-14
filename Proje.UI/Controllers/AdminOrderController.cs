using Microsoft.AspNetCore.Mvc;
using Proje.UI.Services;

namespace Proje.UI.Controllers.Admin
{
    [Route("Admin/Order")]
    public class AdminOrderController : Controller
    {
        private readonly OrderService _orderService;

        public AdminOrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Siparişlerin listelendiği sayfa
            return View("~/Views/Admin/Order/Index.cshtml");
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync();
                return Json(new { success = true, data = orders });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Order/Details.cshtml", order);
        }

        [HttpPost("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, status);
                return Json(new { success = result.Status, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}