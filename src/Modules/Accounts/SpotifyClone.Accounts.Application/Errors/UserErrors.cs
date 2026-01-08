using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Accounts.Application.Errors;

public static class UserErrors
{
    public static readonly Error InvalidAvatarImage = new(
        "User.InvalidAvatarImage",
        "The provided avatar image is invalid.");

    public static readonly Error InvalidBirthDate = new(
        "User.InvalidBirthDate",
        "The provided birth date is invalid.");

    public static readonly Error InvalidDisplayName = new(
        "User.InvalidDisplayName",
        "The provided display name is invalid.");
}
