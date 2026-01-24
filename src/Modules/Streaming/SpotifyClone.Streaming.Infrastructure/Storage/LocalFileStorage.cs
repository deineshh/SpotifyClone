using Microsoft.AspNetCore.Hosting;
using SpotifyClone.Streaming.Application.Abstractions.Services;

namespace SpotifyClone.Streaming.Infrastructure.Storage;

public class LocalFileStorage(IWebHostEnvironment env) : IFileStorage
{
    public string GetFullPath(string relativePath) =>
        Path.Combine(env.WebRootPath, relativePath);

    public string GetAudioRootPath() =>
        Path.Combine(env.WebRootPath, "audio");

    public string GetLocalConversionRootPath() =>
        GetAudioRootPath();

    public async Task SaveAudioFileAsync(Stream stream, string relativePath)
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

    public async Task DeleteAudioFileAsync(string relativePath)
    {
        string fullPath = GetFullPath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public Task DownloadAudioToLocalFileAsync(string objectName, string localPath)
    {
        string sourcePath = GetFullPath(objectName);
        File.Copy(sourcePath, localPath, true);
        return Task.CompletedTask;
    }
}
