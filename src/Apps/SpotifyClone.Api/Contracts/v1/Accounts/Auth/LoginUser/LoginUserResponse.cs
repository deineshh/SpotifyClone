namespace SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginUser;

public sealed record LoginUserResponse(
    Guid UserId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt);
