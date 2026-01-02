namespace SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

public sealed class IdNullDomainException : DomainException
{
    public IdNullDomainException()
        : base("Id cannot be null.")
    {
    }
}
