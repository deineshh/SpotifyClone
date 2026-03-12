using FluentValidation;

namespace SpotifyClone.Accounts.Application.Features.Accounts.Queries.GetById;

public sealed class GetUserByIdQueryValidator
    : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
        => RuleFor(x => x.UserId)
            .NotNull().WithMessage("User ID is required.")
            .NotEqual(Guid.Empty).WithMessage("User ID is required.");
}
