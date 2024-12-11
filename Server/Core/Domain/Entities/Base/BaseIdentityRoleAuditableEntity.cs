namespace Domain.Entities.Base
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.AspNetCore.Identity;

    using Domain.Interfaces;
    using Domain.Events;

    public abstract class BaseIdentityRoleAuditableEntity : IdentityRole, IAuditableEntity
    {
        private readonly List<BaseEvent> _domainEvents = new();

        protected BaseIdentityRoleAuditableEntity(string name) : base(name)
        {
            NormalizedName = name.ToUpperInvariant();
        }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}