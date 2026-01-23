using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;

namespace SpotifyClone.Streaming.Domain.Exceptions;

public sealed class InvalidFileSizeDomainException : StreamingDomainExceptionBase
{
    public InvalidFileSizeDomainException(string message)
        : base(message)
    {
    }
}
