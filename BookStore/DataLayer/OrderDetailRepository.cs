using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        private readonly IDbContextFactory<BookStoreDBContext> _contextFactory;
        public OrderDetailRepository(IDbContextFactory<BookStoreDBContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            if (orderId <= 0) throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));
            using var context = _contextFactory.CreateDbContext();
            return await context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Book)
                .Include(od => od.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByBookIdAsync(int bookId)
        {
            if (bookId <= 0) throw new ArgumentException("Book ID must be greater than zero.", nameof(bookId));
            using var context = _contextFactory.CreateDbContext();
            return await context.OrderDetails
                .Where(od => od.BookId == bookId)
                .Include(od => od.Order)
                .Include(od => od.Book)
                .AsNoTracking()
                .ToListAsync();
        }
    }
} 