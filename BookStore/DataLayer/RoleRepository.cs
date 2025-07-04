using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataLayer
{
    /// <summary>
    /// Repository implementation for Role entity operations.
    /// </summary>
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        private readonly IDbContextFactory<BookStoreDBContext> _contextFactory;
        public RoleRepository(IDbContextFactory<BookStoreDBContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Role name cannot be empty.", nameof(roleName));
            using var context = _contextFactory.CreateDbContext();
            return await context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
 