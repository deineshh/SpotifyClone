using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByGenre;

public sealed class GetAllTracksByGenreQueryValidator
    : AbstractValidator<GetAllTracksByGenreQuery>
{
    public GetAllTracksByGenreQueryValidator()
        => RuleFor(x => x.GenreId)
            .NotNull().WithMessage("Genre ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Genre ID is required.");
}
