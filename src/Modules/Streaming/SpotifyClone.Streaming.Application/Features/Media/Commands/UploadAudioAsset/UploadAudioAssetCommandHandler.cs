using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Services;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Abstractions.Services.Models;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Application.Jobs;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Enums;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

public sealed class UploadAudioAssetCommandHandler(
    IStreamingUnitOfWork unit,
    IMediaService mediaService,
    IFileStorage storage,
    IBackgroundJobService jobService)
    : ICommandHandler<UploadAudioAssetCommand, UploadAudioAssetCommandResult>
{
    private readonly IStreamingUnitOfWork _unit = unit;
    private readonly IMediaService _mediaService = mediaService;
    private readonly IFileStorage _storage = storage;
    private readonly IBackgroundJobService _jobService = jobService;

    public async Task<Result<UploadAudioAssetCommandResult>> Handle(
        UploadAudioAssetCommand request,
        CancellationToken cancellationToken)
    {
        var audioAssetId = AudioAssetId.From(Guid.NewGuid());
        string extension = Path.GetExtension(request.FileName);
        string tempFileName = $"temp/{audioAssetId.Value}{extension}";

        try
        {
            using Stream stream = request.FileStream;
            await _storage.SaveAudioFileAsync(stream, tempFileName);

            string scratchRoot = _storage.GetLocalConversionRootPath();
            if (!Directory.Exists(scratchRoot))
            {
                Directory.CreateDirectory(scratchRoot);
            }

            string localMetaPath = Path.Combine(scratchRoot, $"{audioAssetId.Value}-meta{extension}");
            await _storage.DownloadAudioToLocalFileAsync(tempFileName, localMetaPath);

            AudioMetadata metadata;
            try
            {
                metadata = await _mediaService.GetAudioMetadataAsync(localMetaPath);
            }
            finally
            {
                if (File.Exists(localMetaPath))
                {
                    File.Delete(localMetaPath);
                }
            }

            _jobService.Enqueue<AudioConversionJob>(job
                => job.ProcessAsync(tempFileName, audioAssetId.Value, scratchRoot));

            var audioAsset = AudioAsset.Create(
                audioAssetId,
                metadata.Duration,
                AudioFormat.From(metadata.Format),
                metadata.SizeInBytes,
                false);

            await _unit.AudioAssets.AddAsync(audioAsset, cancellationToken);

            return new UploadAudioAssetCommandResult(audioAssetId.Value);
        }
        catch
        {
            await _storage.DeleteAudioFileAsync(tempFileName);

            return Result.Failure<UploadAudioAssetCommandResult>(MediaErrors.UploadFailed);
        }
    }
}
