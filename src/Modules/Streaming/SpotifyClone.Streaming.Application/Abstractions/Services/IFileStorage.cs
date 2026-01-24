namespace SpotifyClone.Streaming.Application.Abstractions.Services;

public interface IFileStorage
{
    string GetFullPath(string relativePath);

    string GetAudioRootPath();

    string GetLocalConversionRootPath();

    Task SaveAudioFileAsync(Stream stream, string relativePath);

    Task DeleteAudioFileAsync(string relativePath);

    Task DownloadAudioToLocalFileAsync(string objectName, string localPath);
}
