using SpotifyClone.Playlists.Application.Features.Playlists.Queries;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;

namespace SpotifyClone.Playlists.Application.Abstractions.Data;

public interface IPlaylistReadService
{
    Task<PlaylistDetails?> GetDetailsAsync(
        PlaylistId id,
        CancellationToken cancellationToken = default);
}
