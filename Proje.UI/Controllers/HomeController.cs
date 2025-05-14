using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje.Web.Models;
using System.Diagnostics;

namespace Proje.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // Ana sayfa - Ürünleri listele
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToListAsync();

            return View(products);
        }

        // Kategori sayfası - Kategoriye göre ürünleri listele
        public async Task<IActionResult> Category(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // Ürün detay sayfası
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Sepet sayfası
        public IActionResult Cart()
        {
            return View();
        }

        // AJAX: Sepete ürün ekleme
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            if (product.Stock < quantity)
            {
                return Json(new { success = false, message = "Yeterli stok yok." });
            }

            // Oturum değişkeninde sepeti tut
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                // Mevcut öğenin miktarını artır
                cartItem.Quantity += quantity;
            }
            else
            {
                // Yeni öğe ekle
                cartItem = new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    ImageUrl = product.ImageUrl
                };

                cart.Add(cartItem);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new
            {
                success = true,
                message = "Ürün sepete eklendi.",
                cartItemCount = cart.Sum(i => i.Quantity)
            });
        }

        // AJAX: Sepetten ürün çıkarma
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart == null)
            {
                return Json(new { success = false, message = "Sepet boş." });
            }

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Ürün sepette bulunamadı." });
            }

            cart.Remove(cartItem);

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new
            {
                success = true,
                message = "Ürün sepetten çıkarıldı.",
                cartItemCount = cart.Sum(i => i.Quantity),
                cartTotal = cart.Sum(i => i.Quantity * i.UnitPrice)
            });
        }

        // AJAX: Ürün arama
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { success = false, message = "Arama sorgusu boş olamaz." });
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive &&
                           (p.Name.Contains(query) ||
                            p.Description.Contains(query) ||
                            p.Category.Name.Contains(query)))
                .ToListAsync();

            return Json(new { success = true, products });
        }

        // AJAX: Ürün filtreleme
        [HttpGet]
        public async Task<IActionResult> FilterProducts(int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var products = await query.ToListAsync();

            return Json(new { success = true, products });
        }

        // Ürün resmi yükleme sayfası
        [HttpGet]
        public IActionResult UploadProductImage()
        {
            return View();
        }

        // AJAX: Ürün resmi yükleme
        [HttpPost]
        public async Task<IActionResult> UploadProductImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Lütfen bir dosya seçin." });
            }

            // Dosya uzantısını kontrol et
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Sadece resim dosyaları yüklenebilir." });
            }

            // Benzersiz dosya adı oluştur
            var fileName = Guid.NewGuid().ToString() + extension;
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "products");

            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { success = true, fileName, filePath = "/uploads/products/" + fileName });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // Sepet öğesi modeli (Oturum için)
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

    // Session için uzantı metotları
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }
}