using SpotifyClone.Catalog.Application.Features.Genres.Queries;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;

namespace SpotifyClone.Catalog.Application.Abstractions.Data;

public interface IGenreReadService
{
    Task<GenreDetailsResponse?> GetDetailsAsync(
        GenreId id,
        CancellationToken cancellationToken = default);
}
