using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users.Exceptions;

public sealed class InvalidGenderDomainException : DomainExceptionBase
{
    public InvalidGenderDomainException(string message)
        : base(message)
    {
    }
}
