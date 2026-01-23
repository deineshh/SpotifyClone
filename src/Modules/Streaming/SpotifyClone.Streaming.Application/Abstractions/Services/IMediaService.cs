using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Streaming.Application.Abstractions.Services;

public interface IMediaService
{
    Task<Result> ConvertToHlsAsync(
        string sourceFilePath,
        string outputFolder,
        Guid audioId);
}
