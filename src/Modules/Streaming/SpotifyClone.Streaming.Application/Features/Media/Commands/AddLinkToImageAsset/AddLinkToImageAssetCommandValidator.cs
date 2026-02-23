using FluentValidation;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.AddLinkToImageAsset;

public sealed class AddLinkToImageAssetCommandValidator
    : AbstractValidator<AddLinkToImageAssetCommand>
{
    public AddLinkToImageAssetCommandValidator()
        => RuleFor(x => x.ImageAssetId)
            .NotNull().WithMessage("Image asset ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Image asset ID is required.");
}
