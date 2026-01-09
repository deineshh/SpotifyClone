using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Auth;

internal sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public UserId UserId { get; private set; } = default!;

    public string TokenHash { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsActive
        => RevokedAt == null && ExpiresAt > DateTimeOffset.UtcNow;

    private RefreshToken()
    {
    }

    private RefreshToken(Guid id, UserId userId, string tokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(UserId userId, string rawToken, DateTimeOffset now, TimeSpan lifetime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);

        return new RefreshToken(
            id: Guid.NewGuid(),
            userId: userId,
            tokenHash: Sha256TokenHasher.Hash(rawToken),
            createdAt: now,
            expiresAt: now.Add(lifetime));
    }

    public void Revoke(DateTimeOffset revokedAt, string? replacedByRawToken)
    {
        if (RevokedAt is not null)
        {
            return;
        }

        RevokedAt = revokedAt;

        if (!string.IsNullOrWhiteSpace(replacedByRawToken))
        {
            ReplacedByTokenHash = Sha256TokenHasher.Hash(replacedByRawToken);
        }
    }

    public bool Matches(string rawToken)
        => Sha256TokenHasher.Verify(rawToken, TokenHash);
}
