using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Repositories;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Accounts.Infrastructure.Persistence.Accounts;
using SpotifyClone.Accounts.Infrastructure.Persistence.Accounts.Repositories;
using SpotifyClone.Accounts.Infrastructure.Persistence.Auth.Repositories;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence;

namespace SpotifyClone.Accounts.Infrastructure.Persistence;

internal sealed class AccountsEfCoreUnitOfWork(
    AccountsAppDbContext context, UserProfileEfCoreRepository users, RefreshTokenEfCoreRepository refreshTokens)
    : EfCoreUnitOfWorkBase<AccountsAppDbContext>(context), IAccountsUnitOfWork
{
    public IUserProfileRepository Users => users;
    public IRefreshTokenRepository RefreshTokens => refreshTokens;
}
