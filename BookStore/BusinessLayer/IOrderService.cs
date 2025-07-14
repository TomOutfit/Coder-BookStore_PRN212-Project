using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Interface for order-related business operations.
    /// </summary>
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pageIndex, int pageSize, string? searchKeyword = null);
        Task<int> CountOrdersAsync(string? searchKeyword = null);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
        Task<decimal> CalculateTotalRevenueAsync();
        Task<int> CountOrdersByStatusAsync(string status);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<int> CountOrdersByUserAsync(int id);
    }
} 