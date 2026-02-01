using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;

namespace SpotifyClone.Catalog.Domain.Aggregates.Albums;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(
        AlbumId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Album album,
        CancellationToken cancellationToken = default);
}
