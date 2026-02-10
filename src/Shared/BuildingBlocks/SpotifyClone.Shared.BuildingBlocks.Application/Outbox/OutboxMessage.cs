namespace SpotifyClone.Shared.BuildingBlocks.Application.Outbox;

public sealed class OutboxMessage(
    string type,
    string content)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Type { get; init; } = type;
    public string Content { get; init; } = content;
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedOn { get; private set; }
    public string? Error { get; private set; }

    public void MarkAsProcessed() => ProcessedOn = DateTimeOffset.UtcNow;

    public void MarkAsFailed(string error) => Error = error;
}
