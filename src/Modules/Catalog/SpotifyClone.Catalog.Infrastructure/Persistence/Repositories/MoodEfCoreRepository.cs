using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Domain.Aggregates.Moods;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class MoodEfCoreRepository(CatalogAppDbContext context)
    : IMoodRepository
{
    private readonly DbSet<Mood> _moods = context.Moods;

    public async Task AddAsync(
        Mood mood,
        CancellationToken cancellationToken = default)
        => await _moods.AddAsync(mood, cancellationToken);

    public async Task<Mood?> GetByIdAsync(
        MoodId id,
        CancellationToken cancellationToken = default)
        => await _moods.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
