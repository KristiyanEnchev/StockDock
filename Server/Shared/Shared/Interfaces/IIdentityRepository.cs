namespace Shared.Interfaces
{
    using Domain.Entities;

    public interface IIdentityRepository<TEntity> where TEntity : BaseIdentityAuditableEntity
    {
        Task<TDto?> GetByIdAsync<TDto>(string id, CancellationToken cancellationToken = default) where TDto : class;
        Task<IReadOnlyList<TDto>> GetAllAsync<TDto>(CancellationToken cancellationToken = default) where TDto : class;
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IQueryable<TEntity> AsNoTracking();
        IQueryable<TEntity> AsTracking();
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}