using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

public sealed record UploadAudioAssetCommand(
    string Title,
    string Artist,
    string FileName,
    Stream FileStream)
    : IPersistentCommand<UploadAudioAssetCommandResult>;
