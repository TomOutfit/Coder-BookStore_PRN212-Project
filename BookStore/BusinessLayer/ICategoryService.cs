using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Interface for category-related business operations.
    /// </summary>
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(string? searchKeyword = null);
        Task<int> CountCategoriesAsync(string? searchKeyword = null);
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<bool> AddCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
    }
} 