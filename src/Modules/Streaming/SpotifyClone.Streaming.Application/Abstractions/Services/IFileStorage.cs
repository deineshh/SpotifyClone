using SpotifyClone.Streaming.Application.Abstractions.Services.Models;

namespace SpotifyClone.Streaming.Application.Abstractions.Services;

public interface IFileStorage
{
    string GetFullPath(string relativePath);

    string GetImageRootPath();

    string GetAudioRootPath();

    string GetLocalConversionRootPath();

    Task SaveImageFileAsync(Stream stream, string relativePath);

    Task SaveAudioFileAsync(Stream stream, string relativePath);

    Task DeleteAudioFileAsync(string relativePath);

    Task DeleteImageFileAsync(string relativePath);

    Task DownloadAudioToLocalFileAsync(string objectName, string localPath);

    Task DownloadImageToLocalFileAsync(string objectName, string localPath);
}
