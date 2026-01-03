namespace SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

public abstract record DomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
