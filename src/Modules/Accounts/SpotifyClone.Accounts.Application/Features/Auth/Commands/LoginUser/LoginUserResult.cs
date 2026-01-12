namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginUser;

public sealed record LoginUserResult(
    Guid UserId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken);
