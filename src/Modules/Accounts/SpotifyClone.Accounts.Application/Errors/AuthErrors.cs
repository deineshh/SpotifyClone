using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Accounts.Application.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidEmail = new(
        "Auth.InvalidEmail",
        "The provided email is invalid.");

    public static readonly Error InvalidPassword = new(
        "Auth.InvalidPassword",
        "The provided password is invalid.");

    public static readonly Error SignInNotAllowed = new(
        "Auth.SignInNotAllowed",
        "Sign-in is not allowed for this user.");
}
