using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Domain.Aggregates.User.Exceptions;

public sealed class AvatarImageMustSupportTransparencyDomainException : DomainExceptionBase
{
    public AvatarImageMustSupportTransparencyDomainException()
        : base("Avatar image must support transparency.")
    {
    }
}
