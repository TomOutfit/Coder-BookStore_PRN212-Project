using Entities;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            if (orderId <= 0) throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));
            return await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);
        }

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            return await _orderDetailRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _orderDetailRepository.GetAllAsync(1, int.MaxValue, null, null, "Book,Order");
        }
    }
} 