using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;
using SpotifyClone.Streaming.Application.Errors;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

public sealed class UploadAudioAssetCommandHandler(
    IMediaService mediaService,
    IFileStorage storage)
    : ICommandHandler<UploadAudioAssetCommand, UploadAudioAssetCommandResult>
{
    private readonly IMediaService _mediaService = mediaService;
    private readonly IFileStorage _storage = storage;

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

            string sourcePath = _storage.GetFullPath(tempFileName);
            string musicFolder = _storage.GetMusicRootPath();

            Result<string> convertResult = await _mediaService.ConvertToHlsAsync(
                sourcePath, musicFolder, songId);

            if (convertResult.IsFailure)
            {
                return Result.Failure<UploadAudioAssetCommandResult>(convertResult.Errors);
            }

            return new UploadAudioAssetCommandResult(songId);
        }
        finally
        {
            _storage.DeleteFile(tempFileName);
        }
    }
}
