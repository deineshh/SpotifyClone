using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetDetails;

public sealed class GetTrackDetailsByIdQueryValidator
    : AbstractValidator<GetTrackDetailsByIdQuery>
{
    public GetTrackDetailsByIdQueryValidator()
        => RuleFor(x => x.TrackId)
            .NotNull().WithMessage("Track ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Track ID is required.");
}
