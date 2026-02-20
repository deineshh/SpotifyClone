using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.MoveTrack;

internal sealed class MoveTrackInAlbumCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<MoveTrackInAlbumCommand, MoveTrackInAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<MoveTrackInAlbumCommandResult>> Handle(
        MoveTrackInAlbumCommand request,
        CancellationToken cancellationToken)
    {
        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId), cancellationToken);
        if (album is null)
        {
            return Result.Failure<MoveTrackInAlbumCommandResult>(AlbumErrors.NotFound);
        }

        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId), cancellationToken);
        if (track is null)
        {
            return Result.Failure<MoveTrackInAlbumCommandResult>(TrackErrors.NotFound);
        }

        album.MoveTrack(track.Id, request.TargetPositionIndex);

        return new MoveTrackInAlbumCommandResult();
    }
}
