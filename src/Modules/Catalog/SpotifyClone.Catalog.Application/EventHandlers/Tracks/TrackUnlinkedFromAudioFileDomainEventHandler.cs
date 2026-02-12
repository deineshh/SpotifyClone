using MediatR;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;
using SpotifyClone.Shared.BuildingBlocks.Application.Outbox;
using SpotifyClone.Shared.IntegrationEvents.Catalog.Tracks;

namespace SpotifyClone.Catalog.Application.EventHandlers.Tracks;

internal sealed class TrackUnlinkedFromAudioFileDomainEventHandler(
    ICatalogUnitOfWork unit)
    : INotificationHandler<TrackUnlinkedFromAudioFileDomainEvent>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task Handle(
        TrackUnlinkedFromAudioFileDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var integrationEvent = new TrackUnlinkedFromAudioIntegrationEvent(
                notification.AudioFileId.Value);

        var message = OutboxMessage.FromIntegrationEvent(integrationEvent);

        await _unit.OutboxMessages.AddAsync(message, cancellationToken);
        await _unit.Commit(cancellationToken);
    }
}
