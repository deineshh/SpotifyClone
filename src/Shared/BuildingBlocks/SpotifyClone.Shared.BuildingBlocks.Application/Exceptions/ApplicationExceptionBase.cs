namespace SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message)
        : base(message)
        => ArgumentException.ThrowIfNullOrWhiteSpace(message);

    protected ApplicationExceptionBase(string message, Exception innerException)
        : base(message, innerException)
        => ArgumentException.ThrowIfNullOrWhiteSpace(message);
}
