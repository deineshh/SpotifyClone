namespace SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

public abstract class DomainException : Exception
{
    protected DomainException(string message)
        : base(message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Domain exception message is required.");
        }
    }
}
