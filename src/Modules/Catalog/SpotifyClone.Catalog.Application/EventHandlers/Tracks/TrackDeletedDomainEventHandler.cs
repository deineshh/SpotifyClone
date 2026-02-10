using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;
using SpotifyClone.Shared.IntegrationEvents.Catalog.Tracks;

namespace SpotifyClone.Catalog.Application.EventHandlers.Tracks;

internal sealed class TrackDeletedDomainEventHandler(
    IPublisher publisher,
    ILogger<TrackDeletedDomainEventHandler> logger)
    : INotificationHandler<TrackDeletedDomainEvent>
{
    private readonly IPublisher _publisher = publisher;
    private readonly ILogger<TrackDeletedDomainEventHandler> _logger = logger;

    public async Task Handle(
        TrackDeletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing event {DomainEvent}...", typeof(TrackDeletedDomainEvent).Name);

        await _publisher.Publish(
            new TrackDeletedIntegrationEvent(
                notification.AudioFileId.Value),
            cancellationToken);

        _logger.LogInformation("Event {DomainEvent} published...", typeof(TrackDeletedDomainEvent).Name);
    }
}
