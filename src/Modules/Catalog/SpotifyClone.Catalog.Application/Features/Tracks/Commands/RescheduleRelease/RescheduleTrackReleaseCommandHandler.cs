using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.RescheduleRelease;

internal sealed class RescheduleTrackReleaseCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<RescheduleTrackReleaseCommand, RescheduleTrackReleaseCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<RescheduleTrackReleaseCommandResult>> Handle(
        RescheduleTrackReleaseCommand request,
        CancellationToken cancellationToken)
    {
        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            return Result.Failure<RescheduleTrackReleaseCommandResult>(TrackErrors.NotFound);
        }

        track.RescheduleRelease(request.ReleaseDate);

        return new RescheduleTrackReleaseCommandResult();
    }
}
