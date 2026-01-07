using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Domain.Aggregates.User.Exceptions;

public sealed class AvatarImageInvalidShapeDomainException : DomainExceptionBase
{
    public AvatarImageInvalidShapeDomainException()
        : base("Avatar image must be square.")
    {
    }
}
