using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.MarkAsExplicit;

internal sealed class MarkTrackAsExplicitCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<MarkTrackAsExplicitCommand, MarkTrackAsExplicitCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<MarkTrackAsExplicitCommandResult>> Handle(
        MarkTrackAsExplicitCommand request,
        CancellationToken cancellationToken)
    {
        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            return Result.Failure<MarkTrackAsExplicitCommandResult>(TrackErrors.NotFound);
        }

        track.MarkAsExplicit();

        return new MarkTrackAsExplicitCommandResult();
    }
}
