using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnlinkFromAudioFile;

internal sealed class UnlinkTrackFromAudioFileCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<UnlinkTrackFromAudioFileCommandHandler> logger)
    : ICommandHandler<UnlinkTrackFromAudioFileCommand, UnlinkTrackFromAudioFileCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<UnlinkTrackFromAudioFileCommandHandler> _logger = logger;

    public async Task<Result<UnlinkTrackFromAudioFileCommandResult>> Handle(
        UnlinkTrackFromAudioFileCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Unlinking Track {TrackId} from Audio File", request.TrackId);

        Track? track = await _unit.Tracks.GetByIdAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            _logger.LogWarning(
                "Track {TrackId} not found while unlinking Track from Audio File", request.TrackId);

            return Result.Failure<UnlinkTrackFromAudioFileCommandResult>(TrackErrors.NotFound);
        }

        track.UnlinkAudioFile();

        return new UnlinkTrackFromAudioFileCommandResult();
    }
}
