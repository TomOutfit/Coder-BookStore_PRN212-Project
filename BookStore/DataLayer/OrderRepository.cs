using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly IDbContextFactory<BookStoreDBContext> _contextFactory;
        public OrderRepository(IDbContextFactory<BookStoreDBContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            if (userId <= 0) throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
            using var context = _contextFactory.CreateDbContext();
            return await context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) throw new ArgumentException("Status cannot be empty.", nameof(status));
            using var context = _contextFactory.CreateDbContext();
            return await context.Orders
                .Where(o => o.Status == status.Trim())
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .AsNoTracking()
                .ToListAsync();
        }
    }
} 