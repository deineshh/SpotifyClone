namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;

public sealed class InvalidAudioFormatDomainException : StreamingDomainExceptionBase
{
    public InvalidAudioFormatDomainException(string message)
        : base(message)
    {
    }
}
