using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services.Models;

namespace SpotifyClone.Streaming.Application.Abstractions.Services;

public interface IMediaService
{
    Task<AudioMetadata> GetAudioMetadataAsync(
        string filePath);

    Task<Result> ConvertToHlsAsync(
        string sourceFilePath,
        string outputFolder,
        Guid audioId);
}
