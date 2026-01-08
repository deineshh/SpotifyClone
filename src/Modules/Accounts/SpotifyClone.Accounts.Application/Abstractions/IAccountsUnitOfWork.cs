using SpotifyClone.Accounts.Application.Abstractions.Repositories;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;

namespace SpotifyClone.Accounts.Application.Abstractions;

internal interface IAccountsUnitOfWork : IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
}
