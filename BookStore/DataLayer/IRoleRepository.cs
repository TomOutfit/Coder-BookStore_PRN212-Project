using Entities;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Interface for Role-specific repository operations.
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
} 