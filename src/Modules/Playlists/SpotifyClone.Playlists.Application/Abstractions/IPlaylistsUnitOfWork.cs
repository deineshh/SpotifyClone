using SpotifyClone.Playlists.Domain.Aggregates.Playlists;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;

namespace SpotifyClone.Playlists.Application.Abstractions;

public interface IPlaylistsUnitOfWork : IUnitOfWork
{
    IPlaylistRepository Playlists { get; }
}
