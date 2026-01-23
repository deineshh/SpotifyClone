using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Configurations;

internal sealed class AudioAssetEfCoreConfiguration : IEntityTypeConfiguration<AudioAsset>
{
    public void Configure(EntityTypeBuilder<AudioAsset> builder)
    {
        builder.ToTable("audio_assets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(StreamingEfCoreValueConverters.AudioAssetIdConverter)
            .ValueGeneratedNever();

        builder.Property(x => x.Duration)
            .HasColumnName("duration")
            .IsRequired();

        builder.Property(x => x.Format)
            .HasColumnName("format")
            .HasConversion(StreamingEfCoreValueConverters.AudioFormatConverter)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.FileSizeInBytes)
            .HasColumnName("file_size_in_bytes")
            .IsRequired();

        builder.Property(x => x.IsReady)
            .HasColumnName("is_ready")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
