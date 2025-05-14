using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Proje.UI.Middlewares
{
    public class AdminAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Admin sayfalarına erişim kontrolü
            if (path.StartsWith("/Admin"))
            {
                // Giriş yapmış mı kontrol et
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                // Admin rolü var mı kontrol et
                if (!context.User.IsInRole("Admin"))
                {
                    context.Response.Redirect("/Home/AccessDenied");
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class AdminAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAuthMiddleware>();
        }
    }
}