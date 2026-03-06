using MediatR;
using SpotifyClone.Playlists.Application.Abstractions;
using SpotifyClone.Playlists.Application.Abstractions.Repositories;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists;
using SpotifyClone.Playlists.Infrastructure.Persistence.Database;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence;

namespace SpotifyClone.Playlists.Infrastructure.Persistence;

internal sealed class PlaylistsEfCoreUnitOfWork(
    PlaylistsAppDbContext context,
    IPlaylistRepository playlists,
    IOutboxRepository outbox,
    IPublisher publisher)
    : EfCoreUnitOfWorkBase<PlaylistsAppDbContext>(context, publisher),
    IPlaylistsUnitOfWork
{
    public IPlaylistRepository Playlists => playlists;
    public IOutboxRepository OutboxMessages => outbox;
}
