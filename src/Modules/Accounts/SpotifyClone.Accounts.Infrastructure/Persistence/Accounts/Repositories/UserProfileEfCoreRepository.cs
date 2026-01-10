using Microsoft.EntityFrameworkCore;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Accounts.Repositories;

internal sealed class UserProfileEfCoreRepository(
    AccountsAppDbContext context)
    : IUserProfileRepository
{
    private readonly AccountsAppDbContext _context = context;

    public async Task<UserProfile?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public async Task AddAsync(
        UserProfile user,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles.AddAsync(user, cancellationToken);
}
