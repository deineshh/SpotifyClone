using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;

namespace SpotifyClone.Catalog.Application.EventHandlers.Tracks;

internal sealed class TrackMovedInAlbumDomainEventHandler(
    ICatalogUnitOfWork unit,
    ILogger<TrackMovedInAlbumDomainEventHandler> logger)
    : INotificationHandler<TrackMovedInAlbumDomainEvent>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<TrackMovedInAlbumDomainEventHandler> _logger = logger;

    public async Task Handle(
        TrackMovedInAlbumDomainEvent notification,
        CancellationToken cancellationToken)
    {
        if (notification.OldAlbumId is not null)
        {
            Album? oldAlbum = await _unit.Albums.GetByIdAsync(notification.OldAlbumId, cancellationToken);

            if (oldAlbum is null)
            {
                _logger.LogError(
                    "Album {AlbumId} not found while moving Track {TrackId} to a different album",
                    notification.OldAlbumId.Value,
                    notification.TrackId.Value);

                return;
            }

            oldAlbum.RemoveTrack(notification.TrackId);
        }

        Album? newAlbum = await _unit.Albums.GetByIdAsync(notification.NewAlbumId, cancellationToken);

        if (newAlbum is null)
        {
            _logger.LogError(
                "Album {AlbumId} not found while moving Track {TrackId} from a different album",
                notification.NewAlbumId.Value,
                notification.TrackId.Value);

            return;
        }

        newAlbum.AddTrack(notification.TrackId);
        
        await _unit.Commit(cancellationToken);
    }
}
