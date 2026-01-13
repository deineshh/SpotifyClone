namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithRefreshToken;

public sealed record LoginWithRefreshTokenResult(
    string AccessToken,
    string RefreshToken);
