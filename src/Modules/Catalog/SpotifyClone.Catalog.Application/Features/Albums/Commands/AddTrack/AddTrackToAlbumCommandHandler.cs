using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Catalog.Domain.Services;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.AddTrack;

internal sealed class AddTrackToAlbumCommandHandler(
    ICatalogUnitOfWork unit,
    AlbumTrackDomainService albumTrackDomainService)
    : ICommandHandler<AddTrackToAlbumCommand, AddTrackToAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly AlbumTrackDomainService _albumTrackDomainService = albumTrackDomainService;

    public async Task<Result<AddTrackToAlbumCommandResult>> Handle(
        AddTrackToAlbumCommand request,
        CancellationToken cancellationToken)
    {
        var trackId = TrackId.From(request.TrackId);

        Track? track = await _unit.Tracks.GetByIdAsync(
            trackId, cancellationToken);
        if (track is null)
        {
            return Result.Failure<AddTrackToAlbumCommandResult>(TrackErrors.NotFound);
        }

        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId), cancellationToken);
        if (album is null)
        {
            return Result.Failure<AddTrackToAlbumCommandResult>(AlbumErrors.NotFound);
        }

        album.AddTrack(trackId);
        _albumTrackDomainService.TryMarkAlbumAsReadyToPublish(album);
        _albumTrackDomainService.ReevaluateAlbumType(album);

        return new AddTrackToAlbumCommandResult();
    }
}
