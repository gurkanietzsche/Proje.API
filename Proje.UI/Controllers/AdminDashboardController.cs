using Microsoft.AspNetCore.Mvc;
using Proje.UI.Services;

namespace Proje.UI.Controllers.Admin
{
    [Route("Admin/Dashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly OrderService _orderService;

        public AdminDashboardController(
            ProductService productService,
            CategoryService categoryService,
            OrderService orderService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _orderService = orderService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Admin panel dashboard sayfası
            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}