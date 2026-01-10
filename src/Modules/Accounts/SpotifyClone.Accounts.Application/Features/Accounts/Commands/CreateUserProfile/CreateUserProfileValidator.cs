using FluentValidation;

namespace SpotifyClone.Accounts.Application.Features.Accounts.Commands.CreateUserProfile;

internal sealed class CreateUserProfileValidator
    : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull().WithMessage("User id is required.");

        RuleFor(x => x.DisplayName)
            .NotNull().WithMessage("Display name is required.")
            .NotEmpty().WithMessage("Display name is required.");

        RuleFor(x => x.BirthDate)
            .NotNull().WithMessage("Birth date is required.");

        RuleFor(x => x.Gender)
            .NotNull().WithMessage("Gender is required.")
            .NotEmpty().WithMessage("Gender is required.");
    }
}
