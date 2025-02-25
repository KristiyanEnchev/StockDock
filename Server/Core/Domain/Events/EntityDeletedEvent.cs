namespace Domain.Events
{
    public static class EntityDeletedEvent
    {
        public static EntityDeletedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
            => new(entity);
    }

    public class EntityDeletedEvent<TEntity> : BaseEvent
    {
        public EntityDeletedEvent(TEntity entity) => Entity = entity;

        public TEntity Entity { get; }
    }
}