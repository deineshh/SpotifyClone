using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ArtistEfCoreConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("artists");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(CatalogEfCoreValueConverters.ArtistIdConverter)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(ArtistNameRules.MaxLength)
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasColumnName("bio")
            .HasMaxLength(ArtistBioRules.MaxLength);

        builder.Property(x => x.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired();

        builder.OwnsOne(x => x.Avatar, coverBuilder =>
        {
            coverBuilder.Property(x => x.ImageId)
                .HasColumnName("avatar_image_id")
                .HasConversion(CatalogEfCoreValueConverters.ImageIdConverter)
                .IsRequired();

            coverBuilder.OwnsOne(x => x.Metadata, metadataBuilder =>
            {
                metadataBuilder.Property(x => x.Width)
                    .HasColumnName("avatar_metadata_width");

                metadataBuilder.Property(x => x.Height)
                    .HasColumnName("avatar_metadata_height");

                metadataBuilder.Property(x => x.FileType)
                    .HasConversion(CatalogEfCoreValueConverters.ImageFileTypeConverter)
                    .HasColumnName("avatar_metadata_file_type");

                metadataBuilder.Property(x => x.SizeInBytes)
                    .HasColumnName("avatar_metadata_size_in_bytes");
            });
        });

        builder.OwnsOne(x => x.Banner, coverBuilder =>
        {
            coverBuilder.Property(x => x.ImageId)
                .HasColumnName("banner_image_id")
                .HasConversion(CatalogEfCoreValueConverters.ImageIdConverter)
                .IsRequired();

            coverBuilder.OwnsOne(x => x.Metadata, metadataBuilder =>
            {
                metadataBuilder.Property(x => x.Width)
                    .HasColumnName("banner_metadata_width");

                metadataBuilder.Property(x => x.Height)
                    .HasColumnName("banner_metadata_height");

                metadataBuilder.Property(x => x.FileType)
                    .HasConversion(CatalogEfCoreValueConverters.ImageFileTypeConverter)
                    .HasColumnName("banner_metadata_file_type");

                metadataBuilder.Property(x => x.SizeInBytes)
                    .HasColumnName("banner_metadata_size_in_bytes");
            });
        });

        builder.OwnsMany(x => x.Gallery, gb =>
        {
            gb.ToTable("artist_gallery_images");

            gb.WithOwner().HasForeignKey("ArtistId");

            gb.Property<ArtistId>("ArtistId")
                .HasColumnName("artist_id");

            gb.Property(p => p.ImageId)
                .HasColumnName("image_id")
                .HasConversion(CatalogEfCoreValueConverters.ImageIdConverter)
                .IsRequired();

            gb.OwnsOne(p => p.Metadata, mb =>
            {
                mb.Property(m => m.Width).HasColumnName("metadata_width");
                mb.Property(m => m.Height).HasColumnName("metadata_height");
                mb.Property(m => m.SizeInBytes).HasColumnName("metadata_size_in_bytes");
                mb.Property(m => m.FileType)
                    .HasColumnName("metadata_file_type")
                    .HasConversion(CatalogEfCoreValueConverters.ImageFileTypeConverter);
            });
        });

        builder.Ignore(x => x.DomainEvents);
    }
}
