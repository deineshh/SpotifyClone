namespace SpotifyClone.Api.Contracts.v1.Accounts.Auth.RegisterUser;

public sealed record RegisterUserRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
