namespace Persistence.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;

    using Mapster;

    using Persistence.Context;
    
    using Shared.Interfaces;

    public class IdentityRepository<TEntity> : IIdentityRepository<TEntity>
        where TEntity : BaseIdentityAuditableEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger<IdentityRepository<TEntity>> _logger;

        public IdentityRepository(
            ApplicationDbContext context,
            ILogger<IdentityRepository<TEntity>> logger)
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
                _logger.LogError(ex, "Error occurred while getting identity entity by id {Id}", id);
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
                _logger.LogError(ex, "Error occurred while getting all identity entities");
                throw;
            }
        }

        
    }
}