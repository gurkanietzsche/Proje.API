using Proje.UI.Models.DTOs;

namespace Proje.UI.Services
{
    public class OrderService
    {
        private readonly ApiService _apiService;

        public OrderService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<OrderDTO>> GetUserOrdersAsync()
        {
            return await _apiService.GetAsync<IEnumerable<OrderDTO>>("api/Order");
        }

        public async Task<OrderDTO> GetOrderDetailsAsync(int id)
        {
            return await _apiService.GetAsync<OrderDTO>($"api/Order/{id}");
        }

        public async Task<ResultDTO> CreateOrderAsync(OrderCreateDTO orderDto)
        {
            return await _apiService.PostAsync<ResultDTO>("api/Order", orderDto);
        }

        public async Task<ResultDTO> UpdateOrderStatusAsync(int id, string status)
        {
            var updateDto = new { Status = status };
            return await _apiService.PutAsync<ResultDTO>($"api/Order/{id}/status", updateDto);
        }
    }

    public class OrderCreateDTO
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Türkiye";
        public string PaymentMethod { get; set; } = "Kapıda Ödeme";
    }
}