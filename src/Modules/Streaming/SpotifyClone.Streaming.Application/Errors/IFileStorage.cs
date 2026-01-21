namespace SpotifyClone.Streaming.Application.Errors;

public interface IFileStorage
{
    string GetFullPath(string relativePath);

    string GetMusicRootPath();

    Task SaveFileAsync(Stream stream, string relativePath);

    void DeleteFile(string relativePath);
}
