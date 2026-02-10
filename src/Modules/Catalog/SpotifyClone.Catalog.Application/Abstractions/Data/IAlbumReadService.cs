using SpotifyClone.Catalog.Application.Features.Albums.Queries;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;

namespace SpotifyClone.Catalog.Application.Abstractions.Data;

public interface IAlbumReadService
{
    Task<AlbumDetailsResponse?> GetDetailsAsync(
        AlbumId id,
        CancellationToken cancellationToken = default);
}
