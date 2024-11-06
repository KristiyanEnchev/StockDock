namespace Models.User
{
    using Domain.Entities;

    using Mapster;

    public abstract class BaseIdentityAuditableDto<TDto, TEntity> : IMapFrom<TEntity>
    where TDto : class, new()
    where TEntity : BaseIdentityAuditableEntity
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }

        public virtual void Mapping(TypeAdapterConfig config)
        {
            config.NewConfig<TEntity, TDto>()
                .IgnoreNullValues(true)
                .PreserveReference(true)
                .MaxDepth(3);
        }
    }
}