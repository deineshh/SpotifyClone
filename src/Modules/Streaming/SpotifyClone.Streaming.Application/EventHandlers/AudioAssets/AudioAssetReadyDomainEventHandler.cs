using MediatR;
using SpotifyClone.Shared.IntegrationEvents.Streaming.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Events;

namespace SpotifyClone.Streaming.Application.EventHandlers.AudioAssets;

internal sealed class AudioAssetReadyDomainEventHandler(
    IPublisher publisher)
    : INotificationHandler<AudioAssetReadyDomainEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task Handle(
        AudioAssetReadyDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var integrationEvent = new AudioReadyIntegrationEvent(
            notification.TrackId.Value
        );

        await _publisher.Publish(integrationEvent, cancellationToken);
    }
}
