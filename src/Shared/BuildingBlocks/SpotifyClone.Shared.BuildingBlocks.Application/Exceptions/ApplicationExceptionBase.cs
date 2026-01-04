namespace SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message)
        : base(message)
        => ArgumentException.ThrowIfNullOrWhiteSpace(message);
}
