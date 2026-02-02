using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Enums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Converters;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Configurations.Converters;

internal static class CatalogEfCoreValueConverters
{
    public static readonly StronglyTypedIdEfCoreConverter<TrackId, Guid> TrackIdConverter = new(
        v => TrackId.From(v));

    public static readonly StronglyTypedIdEfCoreConverter<AlbumId, Guid> AlbumIdConverter = new(
        v => AlbumId.From(v));

    public static readonly StronglyTypedIdEfCoreConverter<ArtistId, Guid> ArtistIdConverter = new(
        v => ArtistId.From(v));

    public static readonly StronglyTypedIdEfCoreConverter<ImageId, Guid> ImageIdConverter = new(
        v => ImageId.From(v));

    public static readonly StronglyTypedIdEfCoreConverter<GenreId, Guid> GenreIdConverter = new(
        v => GenreId.From(v));

    public static readonly StronglyTypedIdEfCoreConverter<MoodId, Guid> MoodIdConverter = new(
        v => MoodId.From(v));

    public static readonly ValueConverter<AudioFileId?, Guid?> AudioFileIdConverter = new(
        f => f == null ? null : f.Value,
        v => v == null ? null : AudioFileId.From((Guid)v));

    public static readonly ValueConverter<AlbumType?, string?> AlbumTypeConverter = new(
        t => t == null ? null : t.Value,
        v => v == null ? null : AlbumType.From(v));

    public static readonly ValueConverter<ImageFileType?, string?> ImageFileTypeConverter = new(
        t => t == null ? null : t.Value,
        v => v == null ? null : ImageFileType.From(v));
}
