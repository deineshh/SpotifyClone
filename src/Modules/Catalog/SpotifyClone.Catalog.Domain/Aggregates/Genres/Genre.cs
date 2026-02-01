using SpotifyClone.Catalog.Domain.Aggregates.Genres.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Genres;

public sealed class Genre : AggregateRoot<GenreId, Guid>
{
    public string Name { get; private set; } = null!;
    public GenreCoverImage Cover { get; private set; } = null!;

    public static Genre Create(GenreId id, string name, GenreCoverImage cover)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(cover);

        GenreNameRules.Validate(name);

        return new Genre(id, name, cover);
    }

    public void ChangeName(string name)
    {
        GenreNameRules.Validate(name);
        Name = name;
    }

    private Genre(GenreId id, string name, GenreCoverImage cover)
        : base(id)
    {
        Name = name;
        Cover = cover;
    }

    private Genre()
    {
    }
}
