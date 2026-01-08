using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users.Exceptions;

public sealed class InvalidBirthDateDomainException : DomainExceptionBase
{
    public InvalidBirthDateDomainException(string message)
        : base(message)
    {
    }
}
