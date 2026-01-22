using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Services;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Application.Jobs;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

public sealed class UploadAudioAssetCommandHandler(
    IFileStorage storage,
    IBackgroundJobService jobService)
    : ICommandHandler<UploadAudioAssetCommand, UploadAudioAssetCommandResult>
{
    private readonly IFileStorage _storage = storage;
    private readonly IBackgroundJobService _jobService = jobService;

    public async Task<Result<UploadAudioAssetCommandResult>> Handle(
        UploadAudioAssetCommand request,
        CancellationToken cancellationToken)
    {
        var songId = Guid.NewGuid();
        string tempFileName = $"temp/{songId}{Path.GetExtension(request.FileName)}";

        try
        {
            using Stream stream = request.FileStream;
            await _storage.SaveFileAsync(stream, tempFileName);

            string musicFolder = _storage.GetMusicRootPath();

            _jobService.Enqueue<AudioConversionJob>(job =>
                job.ProcessAsync(tempFileName, songId, musicFolder));

            return new UploadAudioAssetCommandResult(songId);
        }
        catch
        {
            return Result.Failure<UploadAudioAssetCommandResult>(MediaErrors.UploadFailed);
        }
    }
}
