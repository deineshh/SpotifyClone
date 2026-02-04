namespace SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;

public abstract record IntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
