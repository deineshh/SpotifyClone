using FluentValidation;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.LinkAudioToTrack;

public sealed class LinkAudioToTrackCommandValidator
    : AbstractValidator<LinkAudioToTrackCommand>
{
    public LinkAudioToTrackCommandValidator()
    {
        RuleFor(x => x.AudioId)
            .NotNull().WithMessage("Audio ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Audio ID is required.");

        RuleFor(x => x.TrackId)
            .NotNull().WithMessage("Track ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Track ID is required.");
    }
}
