namespace Domain.Interfaces
{
    using Domain.Entities.Base;

    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents);
    }
}