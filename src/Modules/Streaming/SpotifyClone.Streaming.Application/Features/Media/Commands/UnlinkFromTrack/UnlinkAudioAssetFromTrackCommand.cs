using SpotifyClone.Streaming.Application.Abstractions;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UnlinkFromTrack;

public sealed record UnlinkAudioAssetFromTrackCommand(
    Guid AudioAssetId)
    : IStreamingPersistentCommand<UnlinkAudioAssetFromTrackCommandResult>;
