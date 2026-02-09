using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnpublishTrack;

public sealed class UnpublishTrackCommandValidator
    : AbstractValidator<UnpublishTrackCommand>
{
    public UnpublishTrackCommandValidator()
        => RuleFor(x => x.TrackId)
            .NotNull().WithMessage("Track ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Track ID is required.");
}
