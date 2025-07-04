using Entities;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService : IService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password are required.");

            var user = await _userRepository.GetUserByUsernameAsync(username.Trim());
            if (user == null || !user.IsActive)
            {
                System.Diagnostics.Debug.WriteLine($"User not found or inactive: {username}");
                return null;
            }

            // Debug: Kiểm tra password
            System.Diagnostics.Debug.WriteLine($"Comparing passwords - Input: '{password}', DB: '{user.Password}'");
            System.Diagnostics.Debug.WriteLine($"Password match: {user.Password == password}");

            // For now, compare plain text password (since database has plain text passwords)
            // In production, use proper password hashing
            if (user.Password != password)
            {
                System.Diagnostics.Debug.WriteLine("Password mismatch - authentication failed");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"Authentication successful for user: {user.Username}");
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(int pageIndex, int pageSize, string? searchKeyword = null)
        {
            if (pageIndex < 1) throw new ArgumentException("Page index must be greater than or equal to 1.", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));

            Expression<Func<User, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                predicate = u => u.Username.ToLower().Contains(searchKeyword) ||
                                u.Email.ToLower().Contains(searchKeyword) ||
                                (u.FirstName != null && u.FirstName.ToLower().Contains(searchKeyword)) ||
                                (u.LastName != null && u.LastName.ToLower().Contains(searchKeyword));
            }

            return await _userRepository.GetAllAsync(
                pageIndex,
                pageSize,
                predicate,
                orderBy: q => q.OrderBy(u => u.Username),
                includeProperties: "RoleNavigation");
        }

        public async Task<int> CountUsersAsync(string? searchKeyword = null)
        {
            Expression<Func<User, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                predicate = u => u.Username.ToLower().Contains(searchKeyword) ||
                                u.Email.ToLower().Contains(searchKeyword) ||
                                (u.FirstName != null && u.FirstName.ToLower().Contains(searchKeyword)) ||
                                (u.LastName != null && u.LastName.ToLower().Contains(searchKeyword));
            }

            return await _userRepository.CountAsync(predicate);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty.", nameof(username));
            return await _userRepository.GetUserByUsernameAsync(username.Trim());
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.", nameof(email));
            return await _userRepository.GetUserByEmailAsync(email.Trim());
        }

        public async Task<bool> AddUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username)) throw new ArgumentException("Username is required.", nameof(user.Username));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email is required.", nameof(user.Email));
            if (string.IsNullOrWhiteSpace(user.Password)) throw new ArgumentException("Password is required.", nameof(user.Password));

            if (await _userRepository.GetUserByUsernameAsync(user.Username) != null)
                throw new InvalidOperationException($"Username {user.Username} already exists.");
            if (await _userRepository.GetUserByEmailAsync(user.Email) != null)
                throw new InvalidOperationException($"Email {user.Email} already exists.");

            var role = await _roleRepository.GetRoleByNameAsync(user.Role)
                ?? throw new InvalidOperationException($"Role {user.Role} does not exist.");

            user.Password = HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.AddAsync(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username)) throw new ArgumentException("Username is required.", nameof(user.Username));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email is required.", nameof(user.Email));

            var existingUser = await _userRepository.GetByIdAsync(user.Id, "Orders")
                ?? throw new KeyNotFoundException($"User with ID {user.Id} not found.");

            var userWithSameUsername = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (userWithSameUsername != null && userWithSameUsername.Id != user.Id)
                throw new InvalidOperationException($"Username {user.Username} already exists for another user.");

            var userWithSameEmail = await _userRepository.GetUserByEmailAsync(user.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != user.Id)
                throw new InvalidOperationException($"Email {user.Email} already exists for another user.");

            var role = await _roleRepository.GetRoleByNameAsync(user.Role)
                ?? throw new InvalidOperationException($"Role {user.Role} does not exist.");

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Address = user.Address;
            existingUser.City = user.City;
            existingUser.State = user.State;
            existingUser.ZipCode = user.ZipCode;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = HashPassword(user.Password);
            }

            await _userRepository.UpdateAsync(existingUser);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id, "Orders")
                ?? throw new KeyNotFoundException($"User with ID {id} not found.");

            var adminRole = await _roleRepository.GetRoleByNameAsync("Admin");
            if (adminRole != null && user.Role == adminRole.Name)
            {
                var adminCount = await _userRepository.CountAsync(u => u.Role == adminRole.Name);
                if (adminCount <= 1)
                    throw new InvalidOperationException("Cannot delete the last admin user.");
            }

            if (user.Orders.Any())
                throw new InvalidOperationException("Cannot delete user with existing orders.");

            await _userRepository.DeleteAsync(id);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync(
            Expression<Func<User, bool>>? filter = null,
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
            string includeProperties = "")
        {
            return await _userRepository.GetAllAsync(1, int.MaxValue, filter, orderBy, includeProperties);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await GetUserByIdAsync(id);
        }

        public async Task AddAsync(User entity)
        {
            await AddUserAsync(entity);
        }

        public async Task UpdateAsync(User entity)
        {
            await UpdateUserAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await DeleteUserAsync(id);
        }

        public async Task DeleteAsync(User entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await DeleteUserAsync(entity.Id);
        }

        public async Task<int> CountAsync(Expression<Func<User, bool>>? filter = null)
        {
            return await _userRepository.CountAsync(filter);
        }

        private string HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            var hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        public User Authenticate(string username, string password)
        {
            return _userRepository.GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _roleRepository.GetRoleByNameAsync(roleName);
        }
    }
} 