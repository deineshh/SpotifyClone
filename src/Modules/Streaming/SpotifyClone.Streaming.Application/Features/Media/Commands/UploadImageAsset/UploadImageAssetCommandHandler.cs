using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Services;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Application.Jobs;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadImageAsset;

internal sealed class UploadImageAssetCommandHandler(
    IStreamingUnitOfWork unit,
    IFileStorage storage,
    IBackgroundJobService jobService)
    : ICommandHandler<UploadImageAssetCommand, UploadImageAssetCommandResult>
{
    private readonly IStreamingUnitOfWork _unit = unit;
    private readonly IFileStorage _storage = storage;
    private readonly IBackgroundJobService _jobService = jobService;

    public async Task<Result<UploadImageAssetCommandResult>> Handle(
        UploadImageAssetCommand request,
        CancellationToken cancellationToken)
    {
        var imageId = ImageId.From(Guid.NewGuid());
        string extension = Path.GetExtension(request.FileName);
        string tempFileName = $"temp/{imageId.Value}{extension}";
        
        try
        {
            using Stream stream = request.FileStream;
            await _storage.SaveImageFileAsync(stream, tempFileName);
            
            string scratchRoot = _storage.GetLocalConversionRootPath();
            if (!Directory.Exists(scratchRoot))
            {
                Directory.CreateDirectory(scratchRoot);
            }
            
            _jobService.Enqueue<ImageConversionJob>(job
                => job.ProcessAsync(tempFileName, imageId.Value, scratchRoot));
        }
        catch
        {
            await _storage.DeleteImageFileAsync(tempFileName);
            
            return Result.Failure<UploadImageAssetCommandResult>(MediaErrors.ImageUploadFailed);
        }
        
        var audioAsset = ImageAsset.Create(imageId, false, null);
        await _unit.ImageAssets.AddAsync(audioAsset, cancellationToken);
        
        return new UploadImageAssetCommandResult(imageId.Value);
    }
}
