using FluentValidation;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithRefreshToken;

internal sealed class LoginWithRefreshTokenValidator
    : AbstractValidator<LoginWithRefreshTokenCommand>
{
    public LoginWithRefreshTokenValidator()
        => RuleFor(x => x.RawToken)
        .NotNull().WithMessage("Refresh token is required.")
        .NotEmpty().WithMessage("Refresh token is required.");
}
