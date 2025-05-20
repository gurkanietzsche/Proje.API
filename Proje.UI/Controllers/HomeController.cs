using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Proje.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["token"]))
            {
                return RedirectToAction("Login");
            }

            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Categories")]
        public IActionResult Categories()
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["token"]))
            {
                return RedirectToAction("Login");
            }

            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Products/{id}")]
        [Route("Products")]
        public IActionResult Products(int id = 0)
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["token"]))
            {
                return RedirectToAction("Login");
            }

            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            ViewBag.CatId = id;
            return View();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Register")]
        public IActionResult Register()
        {
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Profile")]
        public IActionResult Profile()
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["token"]))
            {
                return RedirectToAction("Login");
            }

            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Users")]
        public IActionResult Users()
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["token"]))
            {
                return RedirectToAction("Login");
            }

            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Orders")]
        public IActionResult Orders()
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["token"]))
            {
                return RedirectToAction("Login");
            }

            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }
    }
}