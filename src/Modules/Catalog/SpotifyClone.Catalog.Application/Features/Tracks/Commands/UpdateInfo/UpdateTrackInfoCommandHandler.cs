using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UpdateInfo;

internal sealed class UpdateTrackInfoCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<UpdateTrackInfoCommand, UpdateTrackInfoCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<UpdateTrackInfoCommandResult>> Handle(
        UpdateTrackInfoCommand request,
        CancellationToken cancellationToken)
    {
        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            return Result.Failure<UpdateTrackInfoCommandResult>(TrackErrors.NotFound);
        }

        track.UpdateMainInfo(
            request.Title,
            request.ReleaseDate,
            request.ContainsExplicitContent,
            request.TrackNumber);

        return new UpdateTrackInfoCommandResult();
    }
}
