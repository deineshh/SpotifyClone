using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;

public abstract class StreamingDomainExceptionBase : DomainExceptionBase
{
    protected StreamingDomainExceptionBase(string message)
        : base(message)
    {
    }
}
