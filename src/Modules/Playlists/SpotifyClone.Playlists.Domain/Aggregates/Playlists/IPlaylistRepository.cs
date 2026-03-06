using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;

namespace SpotifyClone.Playlists.Domain.Aggregates.Playlists;

public interface IPlaylistRepository
{
    Task<Playlist?> GetByIdAsync(
        PlaylistId id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Playlist>> GetByIdsAsync(
        IEnumerable<PlaylistId> ids,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Playlist playlist,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Playlist playlist,
        CancellationToken cancellationToken = default);
}
