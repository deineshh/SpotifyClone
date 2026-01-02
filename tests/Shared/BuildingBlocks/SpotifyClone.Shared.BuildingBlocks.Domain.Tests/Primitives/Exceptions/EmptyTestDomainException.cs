using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain.Exceptions;

internal sealed class EmptyTestDomainException : DomainException
{
    public EmptyTestDomainException() : base(string.Empty)
    {
    }
}
