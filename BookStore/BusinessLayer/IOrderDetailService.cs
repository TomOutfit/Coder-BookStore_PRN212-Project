using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Interface for order detail-related business operations.
    /// </summary>
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<OrderDetail?> GetOrderDetailByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetAllAsync();
    }
} 