using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Abstractions.Services;

public interface ITokenService
{
    string GenerateAccessToken(
        UserId userId,
        string email,
        IReadOnlyCollection<string> roles,
        IReadOnlyDictionary<string, string>? claims = null);
}
