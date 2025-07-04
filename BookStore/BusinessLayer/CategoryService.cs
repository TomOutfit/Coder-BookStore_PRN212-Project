using Entities;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(string? searchKeyword = null)
        {
            Expression<Func<Category, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                predicate = c => c.Name.ToLower().Contains(searchKeyword) ||
                                (c.Description != null && c.Description.ToLower().Contains(searchKeyword));
            }

            return await _categoryRepository.GetAllAsync(
                1,
                int.MaxValue,
                predicate,
                orderBy: q => q.OrderBy(c => c.Name));
        }

        public async Task<int> CountCategoriesAsync(string? searchKeyword = null)
        {
            Expression<Func<Category, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                predicate = c => c.Name.ToLower().Contains(searchKeyword) ||
                                (c.Description != null && c.Description.ToLower().Contains(searchKeyword));
            }

            return await _categoryRepository.CountAsync(predicate);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
            return await _categoryRepository.GetCategoryByNameAsync(name.Trim());
        }

        public async Task<bool> AddCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrWhiteSpace(category.Name)) throw new ArgumentException("Category name is required.", nameof(category.Name));
            if (await _categoryRepository.GetCategoryByNameAsync(category.Name) != null)
            {
                throw new InvalidOperationException($"Category with name {category.Name} already exists.");
            }

            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            await _categoryRepository.AddAsync(category);
            return await _categoryRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrWhiteSpace(category.Name)) throw new ArgumentException("Category name is required.", nameof(category.Name));

            var existingCategory = await _categoryRepository.GetByIdAsync(category.Id)
                ?? throw new KeyNotFoundException($"Category with ID {category.Id} not found.");

            var categoryWithSameName = await _categoryRepository.GetCategoryByNameAsync(category.Name);
            if (categoryWithSameName != null && categoryWithSameName.Id != category.Id)
            {
                throw new InvalidOperationException($"Another category with name {category.Name} already exists.");
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(existingCategory);
            return await _categoryRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

            // Note: If Categories are linked to Books, add check here (requires Book.Category relationship)
            await _categoryRepository.DeleteAsync(id);
            return await _categoryRepository.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync(1, int.MaxValue, null, null, "");
        }
    }
} 