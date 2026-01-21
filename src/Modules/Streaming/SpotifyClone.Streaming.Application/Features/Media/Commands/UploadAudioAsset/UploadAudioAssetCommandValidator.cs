using FluentValidation;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

public sealed class UploadAudioAssetCommandValidator
    : AbstractValidator<UploadAudioAssetCommand>
{
    public UploadAudioAssetCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required.")
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.Artist)
            .NotNull().WithMessage("Artist is requried.")
            .NotEmpty().WithMessage("Artist is requried.");

        RuleFor(x => x.FileName)
            .NotNull().WithMessage("File name is requried.")
            .NotEmpty().WithMessage("File name is requried.");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File stream is requried.");
    }
}
