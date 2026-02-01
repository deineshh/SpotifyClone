using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;

namespace SpotifyClone.Catalog.Domain.Aggregates.Genres;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(
        GenreId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Genre genre,
        CancellationToken cancellationToken = default);
}
