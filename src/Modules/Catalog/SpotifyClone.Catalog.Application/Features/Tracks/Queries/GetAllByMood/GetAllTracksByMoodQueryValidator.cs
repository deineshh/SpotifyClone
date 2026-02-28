using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByMood;

public sealed class GetAllTracksByMoodQueryValidator
    : AbstractValidator<GetAllTracksByMoodQuery>
{
    public GetAllTracksByMoodQueryValidator()
        => RuleFor(x => x.MoodId)
            .NotNull().WithMessage("Mood ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Mood ID is required.");
}
