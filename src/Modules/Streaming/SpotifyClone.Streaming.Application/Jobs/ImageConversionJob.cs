using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

namespace SpotifyClone.Streaming.Application.Jobs;

public sealed class ImageConversionJob(
    IStreamingUnitOfWork unit,
    IMediaService mediaService,
    IFileStorage storage,
    ILogger<ImageConversionJob> logger)
{
    private readonly IStreamingUnitOfWork _unit = unit;
    private readonly IMediaService _mediaService = mediaService;
    private readonly IFileStorage _storage = storage;
    private readonly ILogger<ImageConversionJob> _logger = logger;

    public async Task ProcessAsync(string tempRelativePath, Guid imageId, string outputFolder)
    {
        _logger.LogInformation("Starting background conversion for {ImageId}", imageId);

        string extension = Path.GetExtension(tempRelativePath);
        string workDir = Path.Combine(outputFolder, imageId.ToString());
        string sourcePath = Path.Combine(workDir, "source" + extension);
        string relativePath;
        string localConvertedFilePath = string.Empty;
        Abstractions.Services.Models.ImageMetadata? metadata = null;

        if (!Directory.Exists(workDir))
        {
            Directory.CreateDirectory(workDir);
        }

        try
        {
            await _storage.DownloadImageToLocalFileAsync(tempRelativePath, sourcePath);

            Result result = await _mediaService.ConvertToWebpAsync(sourcePath, outputFolder, imageId);
            if (result.IsFailure)
            {
                _logger.LogError("Conversion failed: {Error}", result.Errors[0].Description);
                throw new Exception($"Conversion failed: {result.Errors[0].Description}");
            }

            string specificImageFolder = Path.Combine(outputFolder, imageId.ToString());
            string[] files = Directory.GetFiles(specificImageFolder, "*", SearchOption.AllDirectories);
            foreach (string fullPath in files)
            {
                if (string.Equals(fullPath, sourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                localConvertedFilePath = fullPath;
                string relative = Path.GetRelativePath(specificImageFolder, fullPath);
                relativePath = $"{imageId}/{relative.Replace(Path.DirectorySeparatorChar, '/')}";
                await using FileStream fs = File.OpenRead(fullPath);
                await _storage.SaveImageFileAsync(fs, relativePath);
            }

            metadata = await _mediaService.GetImageMetadataAsync(localConvertedFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in background job while converting");
            throw;
        }
        finally
        {
            if (Directory.Exists(workDir))
            {
                Directory.Delete(workDir, true);
            }

            await _storage.DeleteImageFileAsync(tempRelativePath);
        }

        if (metadata is null)
        {
            _logger.LogError("Failed to get metadata from {ImageId}", imageId);
            throw new Exception($"Failed to get metadata from {imageId}");
        }

        ImageAsset? imageAsset = await _unit.ImageAssets.GetByIdAsync(ImageId.From(imageId));
        if (imageAsset is null)
        {
            _logger.LogError("Image asset not found for {ImageId}", imageId);
            throw new Exception($"Image asset not found for {imageId}");
        }

        imageAsset.MarkAsReady(new(
            metadata.Width,
            metadata.Height,
            2048,
            2048,
            ImageFileType.From(metadata.FileType),
            metadata.SizeInBytes));

        await _unit.Commit();

        _logger.LogInformation("Job finished successfully for {ImageId}", imageId);
    }
}
