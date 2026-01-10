namespace SpotifyClone.Accounts.Infrastructure.Auth;

public sealed record JwtOptions(
    string Issuer,
    string Audience,
    string SecretKey,
    int AccessTokenLifetimeMinutes,
    int RefreshTokenLifetimeDays)
{
    public const string SectionName = "Jwt";
}
