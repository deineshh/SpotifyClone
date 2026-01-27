using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadImageAsset;

public sealed record UploadImageAssetCommand(
    string FileName,
    Stream FileStream)
    : IPersistentCommand<UploadImageAssetCommandResult>;
