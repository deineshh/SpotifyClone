using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.CorrectTitle;

internal sealed class CorrectTrackTitleCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<CorrectTrackTitleCommand, CorrectTrackTitleCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<CorrectTrackTitleCommandResult>> Handle(
        CorrectTrackTitleCommand request,
        CancellationToken cancellationToken)
    {
        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            return Result.Failure<CorrectTrackTitleCommandResult>(TrackErrors.NotFound);
        }

        track.CorrectTitle(request.Title);

        return new CorrectTrackTitleCommandResult();
    }
}
