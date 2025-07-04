using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataLayer
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        private readonly IDbContextFactory<BookStoreDBContext> _contextFactory;
        public CategoryRepository(IDbContextFactory<BookStoreDBContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
            using var context = _contextFactory.CreateDbContext();
            return await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name.Trim());
        }
    }
} 