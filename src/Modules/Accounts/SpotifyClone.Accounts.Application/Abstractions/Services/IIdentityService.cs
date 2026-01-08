using SpotifyClone.Accounts.Application.Abstractions.Services.Models;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Abstractions.Services;

public interface IIdentityService
{
    Task<Result<IdentityUserInfo>> ValidateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetUserRolesAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<bool> IsTwoFactorEnabledAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
