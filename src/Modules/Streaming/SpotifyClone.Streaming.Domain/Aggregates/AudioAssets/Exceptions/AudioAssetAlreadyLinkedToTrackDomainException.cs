using SpotifyClone.Streaming.Domain.Exceptions;

namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;

public sealed class AudioAssetAlreadyLinkedToTrackDomainException(string message)
    : StreamingDomainExceptionBase(message);
