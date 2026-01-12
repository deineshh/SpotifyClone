namespace SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginUser;

public sealed record LoginUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
