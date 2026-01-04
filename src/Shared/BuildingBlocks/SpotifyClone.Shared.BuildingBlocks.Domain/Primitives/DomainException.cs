namespace SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

public abstract class DomainException : Exception
{
    protected DomainException(string message)
        : base(message)
        => ArgumentException.ThrowIfNullOrWhiteSpace(message);
}
