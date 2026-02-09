using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.PublishTrack;

public sealed class PublishTrackCommandValidator
    : AbstractValidator<PublishTrackCommand>
{
    public PublishTrackCommandValidator()
    {
        RuleFor(x => x.TrackId)
            .NotNull().WithMessage("Track ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Track ID is required.");

        RuleFor(x => x.ReleaseDate)
            .NotNull().WithMessage("Release date is required.");
    }
}
