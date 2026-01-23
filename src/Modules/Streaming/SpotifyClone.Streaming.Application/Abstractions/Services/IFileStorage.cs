namespace SpotifyClone.Streaming.Application.Abstractions.Services;

public interface IFileStorage
{
    string GetFullPath(string relativePath);

    string GetAudioRootPath();

    Task SaveFileAsync(Stream stream, string relativePath);

    Task DeleteFileAsync(string relativePath);
}
