using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.RemoveLinkFromImageAsset;

internal sealed class RemoveLinkFromImageAssetCommandHandler(
    IStreamingUnitOfWork unit)
    : ICommandHandler<RemoveLinkFromImageAssetCommand, RemoveLinkFromImageAssetCommandResult>
{
    private readonly IStreamingUnitOfWork _unit = unit;

    public async Task<Result<RemoveLinkFromImageAssetCommandResult>> Handle(
        RemoveLinkFromImageAssetCommand request,
        CancellationToken cancellationToken)
    {
        ImageAsset? imageAsset = await _unit.ImageAssets.GetByIdAsync(
            ImageId.From(request.ImageAssetId), cancellationToken);
        if (imageAsset is null)
        {
            return Result.Failure<RemoveLinkFromImageAssetCommandResult>(MediaErrors.MediaAssetNotFound);
        }

        imageAsset.RemoveLink();

        return new RemoveLinkFromImageAssetCommandResult();
    }
}
