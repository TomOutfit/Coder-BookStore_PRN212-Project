using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Repository implementation for Book entity operations.
    /// </summary>
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        private readonly IDbContextFactory<BookStoreDBContext> _contextFactory;
        public BookRepository(IDbContextFactory<BookStoreDBContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Book?> GetBookByISBNAsync(string isbn)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task<int> CountBooksByUserAsync(int userId)
        {
            using var context = _contextFactory.CreateDbContext();
            // Count distinct books purchased by the user
            return await context.OrderDetails
                .Where(od => od.Order != null && od.Order.UserId == userId)
                .Select(od => od.BookId)
                .Distinct()
                .CountAsync();
        }
    }
} 