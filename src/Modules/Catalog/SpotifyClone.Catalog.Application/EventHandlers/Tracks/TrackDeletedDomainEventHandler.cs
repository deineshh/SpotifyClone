using System.Text.Json;
using MediatR;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;
using SpotifyClone.Shared.BuildingBlocks.Application.Outbox;
using SpotifyClone.Shared.IntegrationEvents;
using SpotifyClone.Shared.IntegrationEvents.Catalog.Tracks;

namespace SpotifyClone.Catalog.Application.EventHandlers.Tracks;

internal sealed class TrackDeletedDomainEventHandler(
    ICatalogUnitOfWork unit)
    : INotificationHandler<TrackDeletedDomainEvent>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task Handle(
        TrackDeletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var integrationEvent = new TrackDeletedIntegrationEvent(
                notification.AudioFileId.Value);

        var message = new OutboxMessage(
            IntegrationEventTypeRegistry.GetKeyForType(integrationEvent.GetType()),
            JsonSerializer.Serialize(
            integrationEvent,
            integrationEvent.GetType()));

        await _unit.OutboxMessages.AddAsync(message, cancellationToken);
        await _unit.Commit(cancellationToken);
    }
}
