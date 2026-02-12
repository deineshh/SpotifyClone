using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UpdateInfo;

public sealed class UpdateTrackInfoCommandValidator
    : AbstractValidator<UpdateTrackInfoCommand>
{
    public UpdateTrackInfoCommandValidator()
    {
        RuleFor(x => x.TrackId)
            .NotNull().WithMessage("Track ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Track ID is required.");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("New title is required.")
            .NotEmpty().WithMessage("New title is required.");

        RuleFor(x => x.ReleaseDate)
            .NotNull().WithMessage("Release date is required.");

        RuleFor(x => x.ContainsExplicitContent)
            .NotNull().WithMessage("Contains explicit content flag is required.");

        RuleFor(x => x.TrackNumber)
            .NotNull().WithMessage("Track number is required.");
    }
}
