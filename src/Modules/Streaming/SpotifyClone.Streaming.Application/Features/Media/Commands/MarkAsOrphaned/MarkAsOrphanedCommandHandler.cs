using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.MarkAsOrphaned;

internal sealed class MarkAsOrphanedCommandHandler(
    IStreamingUnitOfWork unit,
    ILogger<MarkAsOrphanedCommandHandler> logger)
    : ICommandHandler<MarkAsOrphanedCommand, MarkAsOrphanedCommandResult>
{
    private readonly IStreamingUnitOfWork _unit = unit;
    private readonly ILogger<MarkAsOrphanedCommandHandler> _logger = logger;

    public async Task<Result<MarkAsOrphanedCommandResult>> Handle(
        MarkAsOrphanedCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Marking Audio Asset {AudioAssetId} as orphaned", request.AudioAssetId);

        AudioAsset? audioAsset = await _unit.AudioAssets.GetByIdAsync(
            AudioAssetId.From(request.AudioAssetId),
            cancellationToken);

        if (audioAsset is null)
        {
            _logger.LogWarning(
                "Audio Asset {AudioAssetId} not found while marking as orphaned",
                request.AudioAssetId);

            return Result.Failure<MarkAsOrphanedCommandResult>(MediaErrors.MediaAssetNotFound);
        }

        audioAsset.MarkAsOrphaned();

        return new MarkAsOrphanedCommandResult();
    }
}
