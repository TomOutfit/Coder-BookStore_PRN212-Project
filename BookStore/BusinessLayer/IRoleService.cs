using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Interface for role-related business operations.
    /// </summary>
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<Role?> GetRoleByNameAsync(string name);
    }
} 