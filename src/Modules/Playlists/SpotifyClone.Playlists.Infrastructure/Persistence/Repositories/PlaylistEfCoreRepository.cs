using Microsoft.EntityFrameworkCore;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;
using SpotifyClone.Playlists.Infrastructure.Persistence.Database;

namespace SpotifyClone.Playlists.Infrastructure.Persistence.Repositories;

internal sealed class PlaylistEfCoreRepository(PlaylistsAppDbContext context)
    : IPlaylistRepository
{
    private readonly DbSet<Playlist> _playlists = context.Playlists;

    public async Task AddAsync(
        Playlist playlist,
        CancellationToken cancellationToken = default)
        => await _playlists.AddAsync(playlist, cancellationToken);

    public async Task<Playlist?> GetByIdAsync(
        PlaylistId id,
        CancellationToken cancellationToken = default)
        => await _playlists
            .Where(a => a.Id == id)
            .Include(a => a.Collaborators)
            .Include("_tracks")
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Playlist>> GetByIdsAsync(
        IEnumerable<PlaylistId> ids,
        CancellationToken cancellationToken = default)
    {
        if (!ids.Any())
        {
            return [];
        }

        return await _playlists
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Playlist playlist,
        CancellationToken cancellationToken = default)
        => _playlists.Remove(playlist);
}
