using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.MarkAsNotExplicit;

internal sealed class MarkTrackAsNotExplicitCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<MarkTrackAsNotExplicitCommand, MarkTrackAsNotExplicitCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<MarkTrackAsNotExplicitCommandResult>> Handle(
        MarkTrackAsNotExplicitCommand request,
        CancellationToken cancellationToken)
    {
        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            return Result.Failure<MarkTrackAsNotExplicitCommandResult>(TrackErrors.NotFound);
        }

        track.MarkAsNotExplicit();

        return new MarkTrackAsNotExplicitCommandResult();
    }
}
