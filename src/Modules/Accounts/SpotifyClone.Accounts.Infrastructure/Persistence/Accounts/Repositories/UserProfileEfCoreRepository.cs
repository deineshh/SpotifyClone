using SpotifyClone.Accounts.Domain.Aggregates.Users;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Accounts.Repositories;

internal sealed class UserProfileEfCoreRepository(
    AccountsAppDbContext context)
    : IUserProfileRepository
{
    private readonly AccountsAppDbContext _context = context;

    public async Task AddAsync(
        UserProfile user,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles.AddAsync(user, cancellationToken);
}
