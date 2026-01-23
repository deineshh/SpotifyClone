using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Converters;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Enums;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Configurations.Converters;

internal static class StreamingEfCoreValueConverters
{
    public static readonly StronglyTypedIdEfCoreConverter<AudioAssetId, Guid> AudioAssetIdConverter = new(
        v => AudioAssetId.From(v));

    public static readonly ValueConverter<AudioFormat, string> AudioFormatConverter = new(
        f => f.Value,
        v => AudioFormat.From(v));
}
