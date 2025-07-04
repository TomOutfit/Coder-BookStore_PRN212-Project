using Entities;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Interface for Book-specific repository operations.
    /// </summary>
    public interface IBookRepository : IRepository<Book>
    {
        Task<Book?> GetBookByISBNAsync(string isbn);
    }
} 