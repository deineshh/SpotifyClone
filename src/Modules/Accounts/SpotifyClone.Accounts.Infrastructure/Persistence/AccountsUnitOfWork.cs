using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Repositories;
using SpotifyClone.Accounts.Domain.Aggregates.Users;

namespace SpotifyClone.Accounts.Infrastructure.Persistence;

public sealed class AccountsUnitOfWork : IAccountsUnitOfWork
{
    public IUserProfileRepository Users => throw new NotImplementedException();

    public IRefreshTokenRepository RefreshTokens => throw new NotImplementedException();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
