using FluentValidation;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.RemoveLinkFromImageAsset;

public sealed class RemoveLinkFromImageAssetCommandValidator
    : AbstractValidator<RemoveLinkFromImageAssetCommand>
{
    public RemoveLinkFromImageAssetCommandValidator()
        => RuleFor(x => x.ImageAssetId)
            .NotNull().WithMessage("Image asset ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Image asset ID is required.");
}
