using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Application.Errors;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.AddLinkToImageAsset;

internal sealed class AddLinkToImageAssetCommandHandler(
    IStreamingUnitOfWork unit)
    : ICommandHandler<AddLinkToImageAssetCommand, AddLinkToImageAssetCommandResult>
{
    private readonly IStreamingUnitOfWork _unit = unit;

    public async Task<Result<AddLinkToImageAssetCommandResult>> Handle(
        AddLinkToImageAssetCommand request,
        CancellationToken cancellationToken)
    {
        ImageAsset? imageAsset = await _unit.ImageAssets.GetByIdAsync(
            ImageId.From(request.ImageAssetId), cancellationToken);
        if (imageAsset is null)
        {
            return Result.Failure<AddLinkToImageAssetCommandResult>(MediaErrors.MediaAssetNotFound);
        }

        imageAsset.AddLink();

        return new AddLinkToImageAssetCommandResult();
    }
}
