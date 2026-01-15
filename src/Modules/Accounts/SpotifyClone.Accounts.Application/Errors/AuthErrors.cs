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

    public static readonly Error EmailAlreadyInUse = new(
        "Auth.EmailAlreadyInUse",
        "The specified email is already in use.");

    public static readonly Error RegistrationFailed = new(
        "Auth.RegistrationFailed",
        "Registration of the specified user failed.");

    public static Error Identity(string code, string description) => new(
        $"Identity.{code}",
        description);
}
