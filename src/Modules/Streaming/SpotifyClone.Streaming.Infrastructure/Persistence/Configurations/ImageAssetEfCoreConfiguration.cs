using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;
using SpotifyClone.Streaming.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Configurations;

internal sealed class ImageAssetEfCoreConfiguration : IEntityTypeConfiguration<ImageAsset>
{
    public void Configure(EntityTypeBuilder<ImageAsset> builder)
    {
        builder.ToTable("image_assets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(StreamingEfCoreValueConverters.ImageIdConverter)
            .ValueGeneratedNever();

        builder.Property(x => x.IsReady)
            .HasColumnName("is_ready")
            .IsRequired();

        builder.Property(x => x.IsReady)
            .HasColumnName("is_ready")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.OwnsOne(x => x.Metadata, metadata => metadata.Configure());

        builder.Navigation(x => x.Metadata)
            .IsRequired(false);

        builder.Ignore(x => x.DomainEvents);
    }
}
