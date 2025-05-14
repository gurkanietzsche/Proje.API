using Microsoft.AspNetCore.Mvc;
using Proje.UI.Models.DTOs;
using Proje.UI.Services;
using System.Text.Json;

namespace Proje.UI.Controllers.Admin
{
    [Route("Admin/Category")]
    public class AdminCategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public AdminCategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Kategorilerin listelendiği sayfa
            return View("~/Views/Admin/Category/Index.cshtml");
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Json(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            return View("~/Views/Admin/Category/Create.cshtml");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz kategori bilgileri" });
            }

            try
            {
                var result = await _categoryService.CreateCategoryAsync(categoryDto);
                return Json(new { success = result.Status, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            return View("~/Views/Admin/Category/Edit.cshtml", category);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, CategoryDTO categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return Json(new { success = false, message = "ID uyuşmazlığı" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz kategori bilgileri" });
            }

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(categoryDto);
                return Json(new { success = result.Status, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                return Json(new { success = result.Status, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}