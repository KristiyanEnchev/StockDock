namespace Domain.Events
{
    public static class EntityCreatedEvent
    {
        public static EntityCreatedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
            => new(entity);
    }

    public class EntityCreatedEvent<TEntity> : BaseEvent
    {
        public EntityCreatedEvent(TEntity entity) => Entity = entity;

        public TEntity Entity { get; }
    }
}
