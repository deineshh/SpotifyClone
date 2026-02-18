using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;
using SpotifyClone.Catalog.Domain.Services;

namespace SpotifyClone.Catalog.Application.EventHandlers.Tracks;

internal sealed class TrackMarkedAsReadyToPublishDomainEventHandler(
    ICatalogUnitOfWork unit,
    AlbumTrackDomainService albumTrackDomainService,
    ILogger<TrackMarkedAsReadyToPublishDomainEventHandler> logger)
    : INotificationHandler<TrackMarkedAsReadyToPublishDomainEvent>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly AlbumTrackDomainService _albumTrackDomainService = albumTrackDomainService;
    private readonly ILogger<TrackMarkedAsReadyToPublishDomainEventHandler> _logger = logger;

    public async Task Handle(
        TrackMarkedAsReadyToPublishDomainEvent notification,
        CancellationToken cancellationToken)
    {
        Album? album = await _unit.Albums.GetByIdAsync(notification.AlbumId, cancellationToken);

        if (album is null)
        {
            _logger.LogError(
                "Album {AlbumId} not found while refreshing it's status",
                notification.AlbumId.Value);

            return;
        }

        _albumTrackDomainService.TryMarkAlbumAsReadyToPublish(album);
        _albumTrackDomainService.ReevaluateAlbumType(album);

        await _unit.Commit(cancellationToken);
    }
}
