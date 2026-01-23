using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;

namespace SpotifyClone.Streaming.Application.Jobs;

public class AudioConversionJob(
    IMediaService mediaService,
    IFileStorage storage,
    ILogger<AudioConversionJob> logger)
{
    private readonly IMediaService _mediaService = mediaService;
    private readonly IFileStorage _storage = storage;
    private readonly ILogger<AudioConversionJob> _logger = logger;

    public async Task ProcessAsync(string tempRelativePath, Guid audioId, string outputFolder)
    {
        string sourcePath = _storage.GetFullPath(tempRelativePath);

        try
        {
            _logger.LogInformation("Starting background conversion for {AudioId}", audioId);

            Result result = await _mediaService.ConvertToHlsAsync(sourcePath, outputFolder, audioId);

            if (result.IsFailure)
            {
                logger.LogError("Conversion failed: {Error}", result.Errors[0].Description);
                throw new Exception($"Conversion failed: {result.Errors[0].Description}");
            }

            _logger.LogInformation("Job finished successfully for {AudioId}", audioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in background job");
            throw;
        }
        finally
        {
            await _storage.DeleteFileAsync(sourcePath);
        }
    }
}
