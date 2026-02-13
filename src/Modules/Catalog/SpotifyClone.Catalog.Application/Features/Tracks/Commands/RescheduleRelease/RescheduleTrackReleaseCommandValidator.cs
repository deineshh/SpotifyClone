using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.RescheduleRelease;

public sealed class RescheduleTrackReleaseCommandValidator
    : AbstractValidator<RescheduleTrackReleaseCommand>
{
    public RescheduleTrackReleaseCommandValidator()
    {
        RuleFor(x => x.TrackId)
            .NotNull().WithMessage("Track ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Track ID is required.");

        RuleFor(x => x.ReleaseDate)
            .NotNull().WithMessage("Release date is required.");
    }
}
