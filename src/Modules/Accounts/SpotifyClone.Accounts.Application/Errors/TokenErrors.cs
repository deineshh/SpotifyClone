using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Accounts.Application.Errors;

public static class TokenErrors
{
    public static readonly Error TokenGenerationFailed = new(
        "Token.GenerationFailed",
        "Failed to generate access token.");
}
