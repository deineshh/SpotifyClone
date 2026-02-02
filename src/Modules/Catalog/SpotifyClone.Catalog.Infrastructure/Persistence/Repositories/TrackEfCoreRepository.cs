using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class TrackEfCoreRepository(CatalogAppDbContext context)
    : ITrackRepository
{
    private readonly DbSet<Track> _albums = context.Tracks;

    public async Task AddAsync(
        Track track,
        CancellationToken cancellationToken = default)
        => await _albums.AddAsync(track, cancellationToken);

    public async Task<Track?> GetByIdAsync(
        TrackId id,
        CancellationToken cancellationToken = default)
        => await _albums.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
