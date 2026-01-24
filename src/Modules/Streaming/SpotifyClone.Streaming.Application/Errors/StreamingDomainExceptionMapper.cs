using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;
using SpotifyClone.Streaming.Domain.Exceptions;

namespace SpotifyClone.Streaming.Application.Errors;

public sealed class StreamingDomainExceptionMapper : IDomainExceptionMapper
{
    public bool CanMap(DomainExceptionBase domainException)
        => domainException is StreamingDomainExceptionBase;

    public Error MapToError(DomainExceptionBase domainException)
        => domainException switch
        {
            InvalidFileSizeDomainException => MediaErrors.InvalidFileSize,
            InvalidAudioFormatDomainException => MediaErrors.InvalidFormat,
            InvalidDurationDomainException => MediaErrors.InvalidDuration,
            _ => CommonErrors.Unknown
        };
}
