using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users.Exceptions;

public sealed class InvalidDisplayNameDomainException : DomainExceptionBase
{
    public InvalidDisplayNameDomainException(string message)
        : base(message)
    {
    }
}
