using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Converters;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Enums;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Configurations.Converters;

internal static class StreamingEfCoreValueConverters
{
    public static readonly StronglyTypedIdEfCoreConverter<AudioAssetId, Guid> AudioAssetIdConverter = new(
        v => AudioAssetId.From(v));

    public static readonly ValueConverter<AudioFormat?, string?> AudioFormatConverter = new(
        f => f == null ? null : f.Value,
        v => v == null ? null : AudioFormat.From(v));

    public static readonly StronglyTypedIdEfCoreConverter<ImageId, Guid> ImageIdConverter = new(
        v => ImageId.From(v));

    public static readonly ValueConverter<ImageFileType?, string?> ImageFileTypeConverter = new(
        t => t == null ? null : t.Value,
        v => v == null ? null : ImageFileType.From(v));
}
