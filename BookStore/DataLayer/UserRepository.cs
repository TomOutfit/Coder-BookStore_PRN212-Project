using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataLayer
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly IDbContextFactory<BookStoreDBContext> _contextFactory;
        public UserRepository(IDbContextFactory<BookStoreDBContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public IEnumerable<User> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Users.AsNoTracking().ToList();
        }
    }
} 