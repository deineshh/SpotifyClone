using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.PublishTrack;

internal sealed class PublishTrackCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<PublishTrackCommandHandler> logger)
    : ICommandHandler<PublishTrackCommand, PublishTrackCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<PublishTrackCommandHandler> _logger = logger;

    public async Task<Result<PublishTrackCommandResult>> Handle(
        PublishTrackCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing Track {TrackId}", request.TrackId);

        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            _logger.LogWarning(
                "Track {TrackId} not found while publishing", request.TrackId);

            return Result.Failure<PublishTrackCommandResult>(TrackErrors.NotFound);
        }

        track.Publish(request.ReleaseDate);

        return new PublishTrackCommandResult();
    }
}
