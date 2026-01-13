namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithPassword;

public sealed record LoginWithPasswordResult(
    string AccessToken,
    string RefreshToken);
