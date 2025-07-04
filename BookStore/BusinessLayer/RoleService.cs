using Entities;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync(
                1,
                int.MaxValue,
                filter: q => true,
                orderBy: q => q.OrderBy(r => r.Name));
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
            return await _roleRepository.GetRoleByNameAsync(name.Trim());
        }
    }
} 