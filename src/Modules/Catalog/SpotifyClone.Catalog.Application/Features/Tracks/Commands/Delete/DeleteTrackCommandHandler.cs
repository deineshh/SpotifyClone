using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.Delete;

internal sealed class DeleteTrackCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<DeleteTrackCommandHandler> logger)
    : ICommandHandler<DeleteTrackCommand, DeleteTrackCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<DeleteTrackCommandHandler> _logger = logger;

    public async Task<Result<DeleteTrackCommandResult>> Handle(
        DeleteTrackCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting Track {TrackId}", request.TrackId);

        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            _logger.LogWarning(
                "Track {TrackId} not found while deleting", request.TrackId);

            return Result.Failure<DeleteTrackCommandResult>(TrackErrors.NotFound);
        }

        track.PrepareForDeletion();
        await _unit.Tracks.DeleteAsync(track, cancellationToken);

        return new DeleteTrackCommandResult();
    }
}
