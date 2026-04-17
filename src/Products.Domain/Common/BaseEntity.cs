namespace Products.Domain.Common
{


    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        private readonly List<INotification> _domainEvents = new();

        public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}