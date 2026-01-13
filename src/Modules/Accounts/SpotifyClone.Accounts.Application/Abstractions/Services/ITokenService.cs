using SpotifyClone.Accounts.Application.Abstractions.Services.Models;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Abstractions.Services;

public interface ITokenService
{
    AccessToken GenerateAccessToken(
        UserId userId,
        string email,
        IReadOnlyCollection<string> roles,
        IReadOnlyDictionary<string, string>? claims = null);

    RefreshTokenEnvelope GenerateRefreshToken(UserId userId);
}
