namespace SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithRefreshToken;

public sealed record LoginWithRefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}
