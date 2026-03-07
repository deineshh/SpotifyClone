using SpotifyClone.Playlists.Application.Abstractions.Data;
using SpotifyClone.Playlists.Application.Errors;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Playlists.Application.Features.Playlists.Queries.GetDetails;

internal sealed class GetPlaylistDetailsQueryHandler(
    IPlaylistReadService playlistReadService,
    ICurrentUser currentUser)
    : IQueryHandler<GetPlaylistDetailsQuery, PlaylistDetails>
{
    private readonly IPlaylistReadService _playlistReadService = playlistReadService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<PlaylistDetails>> Handle(
        GetPlaylistDetailsQuery request,
        CancellationToken cancellationToken)
    {
        PlaylistDetails? playlist = await _playlistReadService.GetDetailsAsync(
            PlaylistId.From(request.PlaylistId),
            cancellationToken);
        if (playlist is null)
        {
            return Result.Failure<PlaylistDetails>(PlaylistErrors.NotFound);
        }

        if (!_currentUser.IsAuthenticated || playlist.OwnerId != _currentUser.Id)
        {
            return Result.Failure<PlaylistDetails>(PlaylistErrors.NotFound);
        }

        return playlist;
    }
}
