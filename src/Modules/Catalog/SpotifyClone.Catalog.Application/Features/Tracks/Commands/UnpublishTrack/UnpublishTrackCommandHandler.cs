using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnpublishTrack;

internal sealed class UnpublishTrackCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<UnpublishTrackCommandHandler> logger)
    : ICommandHandler<UnpublishTrackCommand, UnpublishTrackCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<UnpublishTrackCommandHandler> _logger = logger;

    public async Task<Result<UnpublishTrackCommandResult>> Handle(
        UnpublishTrackCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Unpublishing Track {TrackId}", request.TrackId);

        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            _logger.LogWarning(
                "Track {TrackId} not found while unpublishing", request.TrackId);

            return Result.Failure<UnpublishTrackCommandResult>(TrackErrors.NotFound);
        }

        track.Unpublish();

        return new UnpublishTrackCommandResult();
    }
}
