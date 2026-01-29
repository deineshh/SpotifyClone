using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

public sealed record UploadAudioAssetCommand(
    string FileName,
    Stream FileStream)
    : IPersistentCommand<UploadAudioAssetCommandResult>;
