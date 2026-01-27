using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services.Models;

namespace SpotifyClone.Streaming.Application.Abstractions.Services;

public interface IMediaService
{
    Task<AudioMetadata> GetAudioMetadataAsync(string filePath);

    Task<ImageMetadata> GetImageMetadataAsync(string filePath);

    Task<Result> ConvertToHlsDashAsync(
        string sourceFilePath,
        string outputFolder,
        Guid audioId);

    Task<Result> ConvertToWebpAsync(
        string sourceFilePath,
        string outputFolder,
        Guid imageId);
}
