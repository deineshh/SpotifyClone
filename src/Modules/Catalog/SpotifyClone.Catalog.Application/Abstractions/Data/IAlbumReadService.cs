using SpotifyClone.Catalog.Application.Features.Albums.Queries;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;

namespace SpotifyClone.Catalog.Application.Abstractions.Data;

public interface IAlbumReadService
{
    Task<AlbumDetails?> GetDetailsAsync(
        AlbumId id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AlbumSummary>> GetAllByArtistIdAsync(
        ArtistId artistId,
        CancellationToken cancellationToken = default);
}
