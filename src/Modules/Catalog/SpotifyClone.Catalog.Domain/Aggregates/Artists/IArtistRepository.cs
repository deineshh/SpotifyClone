using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;

namespace SpotifyClone.Catalog.Domain.Aggregates.Artists;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(
        ArtistId id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Artist>> GetByIdsAsync(
        IEnumerable<ArtistId> ids,
        CancellationToken cancellationToken = default);

    Task<Artist?> GetBannedByIdAsync(
        ArtistId id,
        CancellationToken cancellationToken = default);

    Task<bool> Exists(
        ArtistId id,
        CancellationToken cancellationToken = default);

    Task<bool> Exists(
        ISet<ArtistId> ids,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Artist artist,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Artist artist,
        CancellationToken cancellationToken = default);
}
