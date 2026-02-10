using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;

namespace SpotifyClone.Catalog.Application.Abstractions.Data;

public interface IArtistReadService
{
    Task<ArtistDetailsResponse?> GetDetailsAsync(
        ArtistId id,
        CancellationToken cancellationToken = default);
}
