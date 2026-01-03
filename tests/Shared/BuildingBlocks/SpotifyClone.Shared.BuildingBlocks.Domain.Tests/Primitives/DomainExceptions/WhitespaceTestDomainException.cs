using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.DomainExceptions;

internal sealed class WhitespaceTestDomainException : DomainException
{
    public WhitespaceTestDomainException() : base("  ")
    {
    }
}
