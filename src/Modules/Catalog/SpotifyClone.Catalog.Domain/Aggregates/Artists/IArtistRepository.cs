using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;

namespace SpotifyClone.Catalog.Domain.Aggregates.Artists;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(
        ArtistId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Artist artist,
        CancellationToken cancellationToken = default);
}
