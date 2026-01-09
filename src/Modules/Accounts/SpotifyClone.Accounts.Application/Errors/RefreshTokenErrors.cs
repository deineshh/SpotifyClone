using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Accounts.Application.Errors;

public static class RefreshTokenErrors
{
    public static readonly Error InvalidToken = new(
        "RefreshToken.InvalidToken",
        "The specified refresh token is invalid.");
}
