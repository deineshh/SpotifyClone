namespace SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

public abstract class AggregateRoot<TId, TIdValue> : Entity<TId, TIdValue>
    where TId : notnull, StronglyTypedId<TIdValue>
    where TIdValue : notnull
{
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot()
        : base()
    {
    }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    protected void RaiseDomainEvent(DomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    protected void ClearDomainEvents()
        => _domainEvents.Clear();
}
