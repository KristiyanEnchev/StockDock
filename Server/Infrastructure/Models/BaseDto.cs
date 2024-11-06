namespace Models
{
    using Mapster;

    public abstract class BaseDto<TDto, TEntity> : IMapFrom<TEntity>
        where TDto : class, new()
        where TEntity : class
    {
        public virtual void Mapping(TypeAdapterConfig config)
        {
            config.NewConfig<TEntity, TDto>()
                .IgnoreNullValues(true)
                .PreserveReference(true)
                .MaxDepth(3)
                .Compile();

            config.NewConfig<TDto, TEntity>()
                .IgnoreNullValues(true)
                .PreserveReference(true)
                .MaxDepth(3)
                .Compile();
        }

        public virtual void CustomizeMapping(TypeAdapterConfig config)
        {
        }

        public TEntity ToEntity() => this.Adapt<TEntity>();
        public TEntity ToEntity(TEntity entity) => (this as TDto).Adapt(entity);
        public static TDto FromEntity(TEntity entity) => entity.Adapt<TDto>();
    }
}
