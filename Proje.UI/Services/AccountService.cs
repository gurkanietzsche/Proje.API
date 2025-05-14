using Proje.UI.Models.DTOs;

namespace Proje.UI.Services
{
    public class AccountService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultDTO> LoginAsync(LoginDTO loginDto)
        {
            var result = await _apiService.PostAsync<ResultDTO>("api/Account/login", loginDto);

            if (result.Status && result.Data != null)
            {
                // Veriyi UserDTO'ya dönüştür
                var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDTO>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(result.Data));

                if (userData != null && !string.IsNullOrEmpty(userData.Token))
                {
                    // Token'ı cookie'ye kaydet
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("JWTToken", userData.Token,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddDays(1)
                        });
                }
            }

            return result;
        }

        public async Task<ResultDTO> RegisterAsync(RegisterDTO registerDto)
        {
            return await _apiService.PostAsync<ResultDTO>("api/Account/register", registerDto);
        }

        public async Task<UserDTO> GetProfileAsync()
        {
            return await _apiService.GetAsync<UserDTO>("api/Account/profile");
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("JWTToken");
        }
    }
}