using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Application.Jobs;

public class AudioConversionJob(
    IStreamingUnitOfWork unit,
    IMediaService mediaService,
    IFileStorage storage,
    ILogger<AudioConversionJob> logger)
{
    private readonly IStreamingUnitOfWork _unit = unit;
    private readonly IMediaService _mediaService = mediaService;
    private readonly IFileStorage _storage = storage;
    private readonly ILogger<AudioConversionJob> _logger = logger;

    public async Task ProcessAsync(string tempRelativePath, Guid audioId, string outputFolder)
    {
        _logger.LogInformation("Starting background conversion for {AudioId}", audioId);

        string extension = Path.GetExtension(tempRelativePath);
        string workDir = Path.Combine(outputFolder, audioId.ToString());
        string sourcePath = Path.Combine(workDir, "source" + extension);

        try
        {
            if (!Directory.Exists(workDir))
            {
                Directory.CreateDirectory(workDir);
            }

            await _storage.DownloadAudioToLocalFileAsync(tempRelativePath, sourcePath);

            Result result = await _mediaService.ConvertToHlsAsync(sourcePath, outputFolder, audioId);
            if (result.IsFailure)
            {
                _logger.LogError("Conversion failed: {Error}", result.Errors[0].Description);
                throw new Exception($"Conversion failed: {result.Errors[0].Description}");
            }

            string specificAudioFolder = Path.Combine(outputFolder, audioId.ToString());
            string[] files = Directory.GetFiles(specificAudioFolder, "*", SearchOption.AllDirectories);
            foreach (string fullPath in files)
            {
                if (string.Equals(fullPath, sourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string relative = Path.GetRelativePath(specificAudioFolder, fullPath);
                string relativeKey = $"{audioId}/{relative.Replace(Path.DirectorySeparatorChar, '/')}";
                await using FileStream fs = File.OpenRead(fullPath);
                await _storage.SaveAudioFileAsync(fs, relativeKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in background job");
            throw;
        }
        finally
        {
            if (Directory.Exists(workDir))
            {
                Directory.Delete(workDir, true);
            }

            await _storage.DeleteAudioFileAsync(tempRelativePath);
        }

        AudioAsset? audioAsset = await _unit.AudioAssets.GetByIdAsync(AudioAssetId.From(audioId));
        if (audioAsset is null)
        {
            _logger.LogError("Audio asset not found for {AudioId}", audioId);
            throw new Exception($"Audio asset not found for {audioId}");
        }

        audioAsset.MarkAsReady();

        await _unit.Commit();

        _logger.LogInformation("Job finished successfully for {AudioId}", audioId);
    }
}
