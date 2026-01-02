using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain.Exceptions;

internal sealed class NullTestDomainException : DomainException
{
    public NullTestDomainException() : base(null!)
    {
    }
}
