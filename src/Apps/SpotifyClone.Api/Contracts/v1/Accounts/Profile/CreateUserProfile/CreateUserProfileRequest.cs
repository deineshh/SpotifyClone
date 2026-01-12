namespace SpotifyClone.Api.Contracts.v1.Accounts.Profile.CreateUserProfile;

public sealed record CreateUserProfileRequest
{
    public required Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public required DateTimeOffset BirthDate { get; set; } // 2005-09-16T10:00:00
    public required string Gender { get; set; }
}
