namespace Persistence.Repositories
{
    using System.Linq.Expressions;

    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;

    using Mapster;

    using Persistence.Context;

    using Shared.Interfaces;
    using Domain.Interfaces;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseAuditableEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger<Repository<TEntity>> _logger;

        public Repository(
            ApplicationDbContext context,
            ILogger<Repository<TEntity>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TDto?> GetByIdAsync<TDto>(string id, CancellationToken cancellationToken = default)
            where TDto : class
        {
            try
            {
                var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
                return entity?.Adapt<TDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting entity by id {Id}", id);
                throw;
            }
        }

        public virtual async Task<TDto?> FirstOrDefaultAsync<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TDto : class
        {
            try
            {
                var entity = await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
                return entity?.Adapt<TDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting first entity with predicate");
                throw;
            }
        }

        public virtual async Task<IReadOnlyList<TDto>> GetAllAsync<TDto>(CancellationToken cancellationToken = default)
            where TDto : class
        {
            try
            {
                var entities = await _dbSet.ToListAsync(cancellationToken);
                return entities.Adapt<IReadOnlyList<TDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all entities");
                throw;
            }
        }

        public virtual async Task<IReadOnlyList<TDto>> FindAsync<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TDto : class
        {
            try
            {
                var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
                return entities.Adapt<IReadOnlyList<TDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding entities with predicate");
                throw;
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _dbSet.AddAsync(entity, cancellationToken);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding entity");
                throw;
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _dbSet.Update(entity);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating entity");
                throw;
            }
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity is ISoftDelete)
                {
                    ((ISoftDelete)entity).IsDeleted = true;
                    await UpdateAsync(entity, cancellationToken);
                }
                else
                {
                    _dbSet.Remove(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting entity");
                throw;
            }
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict detected while saving changes");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes");
                throw;
            }
        }

        public virtual IQueryable<TEntity> AsNoTracking() => _dbSet.AsNoTracking();

        public virtual IQueryable<TEntity> AsTracking() => _dbSet.AsTracking();

        public virtual IQueryable<TEntity> GetAllIncludingDeleted()
        {
            return _dbSet.IgnoreQueryFilters();
        }
    }
}