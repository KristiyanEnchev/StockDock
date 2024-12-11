namespace Domain.Entities.Base
{
    using Domain.Events;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class BaseEntity
    {
        public string Id { get; protected set; }

        private readonly List<BaseEvent> _domainEvents = new();

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();
        public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}