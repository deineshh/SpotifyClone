using MediatR;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Events;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;

namespace SpotifyClone.Catalog.Application.EventHandlers.Albums;

internal sealed class AlbumUnpublishedDomainEventHandler(
    ICatalogUnitOfWork unit)
    : INotificationHandler<AlbumUnpublishedDomainEvent>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task Handle(
        AlbumUnpublishedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        IEnumerable<Track> tracks = await _unit.Tracks.GetAllByAlbumAsync(
            notification.AlbumId,
            cancellationToken);

        foreach (Track track in tracks)
        {
            track.Unpublish();
        }

        await _unit.Commit(cancellationToken);
    }
}
