using Entities;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderDetailRepository = orderDetailRepository ?? throw new ArgumentNullException(nameof(orderDetailRepository));
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int pageIndex, int pageSize, string? searchKeyword = null)
        {
            if (pageIndex < 1) throw new ArgumentException("Page index must be greater than or equal to 1.", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));

            Expression<Func<Order, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                if (int.TryParse(searchKeyword, out int id))
                {
                    predicate = o => o.Id == id || o.UserId == id;
                }
                else
                {
                    predicate = o => o.Status.ToLower().Contains(searchKeyword) ||
                                    (o.ShippingAddress != null && o.ShippingAddress.ToLower().Contains(searchKeyword)) ||
                                    (o.PaymentMethod != null && o.PaymentMethod.ToLower().Contains(searchKeyword)) ||
                                    (o.Notes != null && o.Notes.ToLower().Contains(searchKeyword));
                }
            }

            return await _orderRepository.GetAllAsync(
                1,
                int.MaxValue,
                predicate,
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                includeProperties: "User,OrderDetails.Book");
        }

        public async Task<int> CountOrdersAsync(string? searchKeyword = null)
        {
            Expression<Func<Order, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                if (int.TryParse(searchKeyword, out int id))
                {
                    predicate = o => o.Id == id || o.UserId == id;
                }
                else
                {
                    predicate = o => o.Status.ToLower().Contains(searchKeyword) ||
                                    (o.ShippingAddress != null && o.ShippingAddress.ToLower().Contains(searchKeyword)) ||
                                    (o.PaymentMethod != null && o.PaymentMethod.ToLower().Contains(searchKeyword)) ||
                                    (o.Notes != null && o.Notes.ToLower().Contains(searchKeyword));
                }
            }

            return await _orderRepository.CountAsync(predicate);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            return await _orderRepository.GetByIdAsync(id, "User,OrderDetails.Book");
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            if (userId <= 0) throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
            return await _orderRepository.GetAllAsync(
                1,
                int.MaxValue,
                o => o.UserId == userId,
                orderBy: q => q.OrderByDescending(o => o.OrderDate),
                includeProperties: "User,OrderDetails.Book");
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.TotalAmount < 0) throw new ArgumentException("Total amount cannot be negative.", nameof(order.TotalAmount));

            var existingOrder = await _orderRepository.GetByIdAsync(order.Id, "OrderDetails")
                ?? throw new KeyNotFoundException($"Order with ID {order.Id} not found.");

            existingOrder.UserId = order.UserId;
            existingOrder.Status = order.Status;
            existingOrder.ShippingAddress = order.ShippingAddress;
            existingOrder.PaymentMethod = order.PaymentMethod;
            existingOrder.Notes = order.Notes;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(existingOrder);
            return await _orderRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id, "OrderDetails")
                ?? throw new KeyNotFoundException($"Order with ID {id} not found.");

            if (order.Status != "Pending")
                throw new InvalidOperationException("Only pending orders can be deleted.");

            foreach (var detail in order.OrderDetails)
            {
                await _orderDetailRepository.DeleteAsync(detail.Id);
            }

            await _orderRepository.DeleteAsync(id);
            return await _orderRepository.SaveChangesAsync() > 0;
        }

        public async Task<decimal> CalculateTotalRevenueAsync()
        {
            var orders = await _orderRepository.GetAllAsync(
                1,
                int.MaxValue,
                o => o.Status == "Completed",
                orderBy: null,
                includeProperties: "");
            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<int> CountOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) throw new ArgumentException("Status cannot be empty.", nameof(status));
            return await _orderRepository.CountAsync(o => o.Status == status.Trim());
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync(1, int.MaxValue, null, null, "User,OrderDetails.Book");
        }

        public async Task<int> CountOrdersByUserAsync(int userId)
        {
            return await _orderRepository.CountOrdersByUserAsync(userId);
        }
    }
} 