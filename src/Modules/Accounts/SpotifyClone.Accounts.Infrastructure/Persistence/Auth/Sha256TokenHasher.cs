using System.Security.Cryptography;
using System.Text;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Auth;

internal static class Sha256TokenHasher
{
    public static string Hash(string rawToken)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(rawToken);
        return Convert.ToBase64String(SHA256.HashData(bytes));
    }

    public static bool Verify(string rawToken, string hash)
        => Hash(rawToken) == hash;
}
