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


    }
}