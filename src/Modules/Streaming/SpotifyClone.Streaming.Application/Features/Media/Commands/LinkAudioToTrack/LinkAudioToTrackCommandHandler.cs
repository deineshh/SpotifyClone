using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.LinkAudioToTrack;

internal sealed class LinkAudioToTrackCommandHandler(
    ILogger<LinkAudioToTrackCommandHandler> logger,
    IStreamingUnitOfWork unit)
    : ICommandHandler<LinkAudioToTrackCommand, LinkAudioToTrackCommandResult>
{
    private readonly ILogger<LinkAudioToTrackCommandHandler> _logger = logger;
    private readonly IStreamingUnitOfWork _unit = unit;

    public async Task<Result<LinkAudioToTrackCommandResult>> Handle(
        LinkAudioToTrackCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Linking AudioAsset {AudioId}", request.AudioId);

        AudioAsset? audioAsset = await _unit.AudioAssets.GetByIdAsync(
            AudioAssetId.From(request.AudioId),
            cancellationToken);

        if (audioAsset is null)
        {
            _logger.LogWarning(
                "Audio Asset {AudioId} not found", request.AudioId);

            return Result.Failure<LinkAudioToTrackCommandResult>(MediaErrors.MediaFileNotFound);
        }

        if (audioAsset.TrackId is not null)
        {
            return Result.Failure<LinkAudioToTrackCommandResult>(MediaErrors.AudioAlreadyLinkedToTrack);
        }

        audioAsset.LinkTrack(TrackId.From(request.TrackId));

        return new LinkAudioToTrackCommandResult();
    }
}
