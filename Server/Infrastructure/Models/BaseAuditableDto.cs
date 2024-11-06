namespace Models
{
    using Domain.Entities;

    public abstract class BaseAuditableDto<TDto, TEntity> : BaseDto<TDto, TEntity>
        where TDto : class, new()
        where TEntity : BaseAuditableEntity
    {
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}