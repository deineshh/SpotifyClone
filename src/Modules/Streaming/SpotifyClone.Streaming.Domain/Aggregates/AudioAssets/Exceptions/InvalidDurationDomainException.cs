namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;

public sealed class InvalidDurationDomainException : StreamingDomainExceptionBase
{
    public InvalidDurationDomainException(string message)
        : base(message)
    {
    }
}
