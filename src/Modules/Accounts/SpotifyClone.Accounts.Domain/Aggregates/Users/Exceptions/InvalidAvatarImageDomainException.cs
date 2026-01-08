using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users.Exceptions;

public sealed class InvalidAvatarImageDomainException : DomainExceptionBase
{
    public InvalidAvatarImageDomainException(string message)
        : base(message)
    {
    }
}
