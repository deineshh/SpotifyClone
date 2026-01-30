using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Shared.Kernel.ValueObjects;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;
using SpotifyClone.Streaming.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Configurations;

internal static class ImageAssetMetadataEfCoreConfiguration
{
    public static OwnedNavigationBuilder<ImageAsset, ImageMetadata> Configure(
        this OwnedNavigationBuilder<ImageAsset, ImageMetadata> builder)
    {
        builder.Property(m => m.Width)
            .HasColumnName("metadata_width");

        builder.Property(m => m.Height)
            .HasColumnName("metadata_height");

        builder.Property(m => m.FileType)
            .HasConversion(StreamingEfCoreValueConverters.ImageFileTypeConverter)
            .HasColumnName("metadata_file_type")
            .HasMaxLength(10);

        builder.Property(m => m.SizeInBytes)
            .HasColumnName("metadata_size_in_bytes");

        return builder;
    }
}
