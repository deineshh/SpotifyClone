using SpotifyClone.Streaming.Application.Abstractions;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.RemoveLinkFromImageAsset;

public sealed record RemoveLinkFromImageAssetCommand(
    Guid ImageAssetId)
    : IStreamingPersistentCommand<RemoveLinkFromImageAssetCommandResult>;
