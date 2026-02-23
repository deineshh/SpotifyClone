using SpotifyClone.Streaming.Application.Abstractions;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.AddLinkToImageAsset;

public sealed record AddLinkToImageAssetCommand(
    Guid ImageAssetId)
    : IStreamingPersistentCommand<AddLinkToImageAssetCommandResult>;
