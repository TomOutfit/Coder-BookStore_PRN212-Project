using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Interface for book-related business operations.
    /// </summary>
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(int pageIndex, int pageSize, string? searchKeyword = null);
        Task<int> CountBooksAsync(string? searchKeyword = null);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book?> GetBookByISBNAsync(string isbn);
        Task<bool> AddBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<int> CountBooksInStockAsync();
        Task<IEnumerable<Book>> GetLowStockBooksAsync(int threshold);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<int> CountBooksByUserAsync(int id);
    }
} 