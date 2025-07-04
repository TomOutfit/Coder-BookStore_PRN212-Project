using Entities;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Interface for Category-specific repository operations.
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetCategoryByNameAsync(string name);
    }
} 