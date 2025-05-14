using Microsoft.AspNetCore.Mvc;
using Proje.UI.Models;
using Proje.UI.Services;
using System.Diagnostics;

namespace Proje.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public HomeController(
            ILogger<HomeController> logger,
            ProductService productService,
            CategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products.Take(8)); // Ana sayfada ilk 8 ürünü göster
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana sayfa ürünleri yüklenirken hata oluştu.");
                return View(new List<Models.DTOs.ProductDTO>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}