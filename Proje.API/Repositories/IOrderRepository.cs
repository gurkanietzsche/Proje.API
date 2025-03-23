using Proje.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersWithItemsAsync();
    }
}