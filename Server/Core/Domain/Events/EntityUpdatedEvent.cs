namespace Domain.Events
{
    public static class EntityUpdatedEvent
    {
        public static EntityUpdatedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
            => new(entity);
    }

    public class EntityUpdatedEvent<TEntity> : BaseEvent
    {
        public EntityUpdatedEvent(TEntity entity) => Entity = entity;

        public TEntity Entity { get; }
    }
}
