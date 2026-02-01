using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;

namespace SpotifyClone.Catalog.Domain.Aggregates.Moods;

public interface IMoodRepository
{
    Task<Mood?> GetByIdAsync(
        MoodId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Mood mood,
        CancellationToken cancellationToken = default);
}
