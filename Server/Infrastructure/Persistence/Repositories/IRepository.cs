namespace Persistence.Repositories
{
    using System.Linq.Expressions;

    using Domain.Entities;

    public interface IRepository<TEntity> where TEntity : BaseAuditableEntity
    {
        Task<TDto?> GetByIdAsync<TDto>(string id, CancellationToken cancellationToken = default) where TDto : class;
        Task<TDto?> FirstOrDefaultAsync<TDto>(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TDto : class;
        Task<IReadOnlyList<TDto>> GetAllAsync<TDto>(CancellationToken cancellationToken = default) where TDto : class;
        Task<IReadOnlyList<TDto>> FindAsync<TDto>(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TDto : class;

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IQueryable<TEntity> AsNoTracking();
        IQueryable<TEntity> AsTracking();
        IQueryable<TEntity> GetAllIncludingDeleted();
    }
}