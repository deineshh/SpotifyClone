using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Accounts.Application.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "The provided credentials are invalid.");

    public static readonly Error TwoFactorRequired = new(
        "Auth.TwoFactorRequired",
        "Two-factor authentication is required.");

    public static readonly Error AccountLocked = new(
        "Auth.AccountLocked",
        "The account is locked.");

    public static readonly Error RefreshTokenInvalid = new(
        "Auth.RefreshTokenInvalid",
        "Refresh token is invalid or expired.");
}
