using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Interface for user-related business operations.
    /// </summary>
    public interface IUserService : IService<User>
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<IEnumerable<User>> GetAllUsersAsync(int pageIndex, int pageSize, string? searchKeyword = null);
        Task<int> CountUsersAsync(string? searchKeyword = null);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> GetUserByIdAsync(int id);
        User Authenticate(string username, string password);
    }
} 