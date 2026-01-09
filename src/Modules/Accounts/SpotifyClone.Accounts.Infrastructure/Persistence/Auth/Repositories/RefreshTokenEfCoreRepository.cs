using Microsoft.EntityFrameworkCore;
using SpotifyClone.Accounts.Application.Abstractions.Repositories;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Auth.Repositories;

internal sealed class RefreshTokenEfCoreRepository : IRefreshTokenRepository
{
    private readonly DbSet<RefreshToken> _refreshTokens;

    public RefreshTokenEfCoreRepository(DbSet<RefreshToken> refreshTokens)
        => _refreshTokens = refreshTokens;

    public async Task<Result<bool>> IsValidAsync(
        UserId userId,
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return Result.Failure<bool>(AuthErrors.InvalidToken);
        }

        bool isValid = await _refreshTokens
            .AsNoTracking()
            .AnyAsync(
                t => t.UserId == userId
                     && t.TokenHash == tokenHash
                     && t.RevokedAt == null
                     && t.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);

        return Result.Success(isValid);
    }

    public async Task<Result> StoreAsync(
        UserId userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return Result.Failure(AuthErrors.InvalidToken);
        }

        var token = new RefreshToken(
            userId,
            tokenHash,
            DateTimeOffset.UtcNow,
            expiresAt);

        await _refreshTokens.AddAsync(token, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RevokeAsync(
        UserId userId,
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return Result.Failure(AuthErrors.InvalidToken);
        }

        RefreshToken? token = await _refreshTokens
            .FirstOrDefaultAsync(
                t => t.UserId == userId && t.TokenHash == tokenHash,
                cancellationToken);

        if (token is null)
        {
            return Result.Success();
        }

        token.Revoke(DateTimeOffset.UtcNow, replacedByTokenHash: null);

        return Result.Success();
    }

    public async Task<Result> RevokeAllAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        List<RefreshToken> tokens = await _refreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        foreach (RefreshToken token in tokens)
        {
            token.Revoke(now, replacedByTokenHash: null);
        }

        return Result.Success();
    }
}

