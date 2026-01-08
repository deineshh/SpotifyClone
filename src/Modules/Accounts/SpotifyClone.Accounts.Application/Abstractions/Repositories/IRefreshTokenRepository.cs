using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task StoreAsync(
        UserId userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default);

    Task<bool> IsValidAsync(
        UserId userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        UserId userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task RevokeAllAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
