using Microsoft.AspNetCore.Mvc;
using Proje.UI.Services;

namespace Proje.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductController(
            ProductService productService,
            CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;

                if (categoryId.HasValue)
                {
                    var products = await _productService.GetProductsByCategoryIdAsync(categoryId.Value);
                    var category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);
                    ViewBag.CategoryName = category?.Name;
                    return View(products);
                }
                else
                {
                    var products = await _productService.GetAllProductsAsync();
                    return View(products);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Models.DTOs.ProductDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}