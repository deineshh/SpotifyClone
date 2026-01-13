namespace SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithPassword;

public sealed record LoginWithPasswordRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
