using Microsoft.AspNetCore.Hosting;
using SpotifyClone.Streaming.Application.Abstractions.Services;

namespace SpotifyClone.Streaming.Infrastructure.Storage;

public class LocalFileStorage(IWebHostEnvironment env) : IFileStorage
{
    private readonly IWebHostEnvironment _env = env;

    public string GetFullPath(string relativePath) =>
        Path.Combine(_env.WebRootPath, relativePath);

    public string GetAudioRootPath() =>
        Path.Combine(_env.WebRootPath, "audio");

    public string GetImageRootPath() =>
        Path.Combine(_env.WebRootPath, "images");

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

    public async Task SaveImageFileAsync(Stream stream, string relativePath)
        => await SaveAudioFileAsync(stream, relativePath);

    public async Task DeleteAudioFileAsync(string relativePath)
    {
        string fullPath = GetFullPath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public async Task DeleteImageFileAsync(string relativePath)
        => await DeleteAudioFileAsync(relativePath);

    public Task DownloadAudioToLocalFileAsync(string objectName, string localPath)
    {
        string sourcePath = GetFullPath(objectName);
        File.Copy(sourcePath, localPath, true);
        return Task.CompletedTask;
    }

    public Task DownloadImageToLocalFileAsync(string objectName, string localPath)
        => DownloadAudioToLocalFileAsync(objectName, localPath);
}
