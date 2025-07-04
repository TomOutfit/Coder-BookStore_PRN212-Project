using Entities;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Interface for User-specific repository operations.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        IEnumerable<User> GetAll();
    }
} 