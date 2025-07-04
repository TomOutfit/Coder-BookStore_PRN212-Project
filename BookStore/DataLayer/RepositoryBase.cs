using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Base implementation for generic repository operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IDbContextFactory<BookStoreDBContext> _contextFactory;

        protected RepositoryBase(IDbContextFactory<BookStoreDBContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id, string? includeProperties = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            try
            {
                if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));

                IQueryable<TEntity> query = dbSet;

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
                    {
                        if (!string.IsNullOrEmpty(includeProperty))
                            query = query.Include(includeProperty);
                    }
                }

                // Kiểm tra entity có property "Id" không
                var idProp = typeof(TEntity).GetProperty("Id");
                if (idProp == null)
                    throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have an 'Id' property.");

                return await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Error retrieving entity of type {typeof(TEntity).Name} with ID {id}: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Unexpected error occurred while retrieving entity of type {typeof(TEntity).Name} with ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string? includeProperties = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            try
            {
                IQueryable<TEntity> query = dbSet;

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
                    {
                        query = query.Include(includeProperty);
                    }
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Error retrieving all entities of type {typeof(TEntity).Name}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error occurred while retrieving all entities of type {typeof(TEntity).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string? includeProperties = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            try
            {
                if (pageIndex < 1) throw new ArgumentException("Page index must be greater than or equal to 1.", nameof(pageIndex));
                if (pageSize < 1) throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));

                IQueryable<TEntity> query = dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
                    {
                        query = query.Include(includeProperty);
                    }
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                return await query.AsNoTracking().ToListAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Error retrieving paginated entities of type {typeof(TEntity).Name}: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Unexpected error occurred while retrieving paginated entities of type {typeof(TEntity).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string? includeProperties = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            try
            {
                IQueryable<TEntity> query = dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
                    {
                        query = query.Include(includeProperty);
                    }
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Error retrieving filtered entities of type {typeof(TEntity).Name}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error occurred while retrieving filtered entities of type {typeof(TEntity).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            await dbSet.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            dbSet.UpdateRange(entities);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            var entity = await dbSet.FindAsync(id);
            if (entity != null)
            {
                dbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            dbSet.RemoveRange(entities);
            await context.SaveChangesAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            // Không cần thiết nữa vì mỗi thao tác đã save riêng
            return 0;
        }
    }
} 