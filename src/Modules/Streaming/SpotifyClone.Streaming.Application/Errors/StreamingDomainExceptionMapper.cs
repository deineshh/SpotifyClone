using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.Exceptions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;

namespace SpotifyClone.Streaming.Application.Errors;

public sealed class StreamingDomainExceptionMapper : IDomainExceptionMapper
{
    public Error MapToError(DomainExceptionBase domainException)
        => domainException switch
        {
            InvalidAudioFormatDomainException => MediaErrors.InvalidFormat,
            InvalidDurationDomainException => MediaErrors.InvalidDuration,
            InvalidImageMetadataDomainException => MediaErrors.InvalidImageMetadata,
            _ => CommonErrors.Unknown
        };
}
