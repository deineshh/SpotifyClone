using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<Result> StoreAsync(
        UserId userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> IsValidAsync(
        UserId userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeAsync(
        UserId userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeAllAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
