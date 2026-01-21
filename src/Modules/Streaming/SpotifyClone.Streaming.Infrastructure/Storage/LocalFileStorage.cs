using Microsoft.AspNetCore.Hosting;
using SpotifyClone.Streaming.Application.Errors;

namespace SpotifyClone.Streaming.Infrastructure.Storage;

public class LocalFileStorage(IHostingEnvironment env) : IFileStorage
{
    public string GetFullPath(string relativePath) =>
        Path.Combine(env.WebRootPath, relativePath);

    public string GetMusicRootPath() =>
        Path.Combine(env.WebRootPath, "music");

    public async Task SaveFileAsync(Stream stream, string relativePath)
    {
        string fullPath = GetFullPath(relativePath);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
    }

    public void DeleteFile(string relativePath)
    {
        string fullPath = GetFullPath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
