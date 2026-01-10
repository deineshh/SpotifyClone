namespace SpotifyClone.Accounts.Domain.Aggregates.Users;

public interface IUserProfileRepository
{
    Task AddAsync(
        UserProfile user,
        CancellationToken cancellationToken = default);
}
