using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class AlbumEfCoreConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("albums");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(CatalogEfCoreValueConverters.AlbumIdConverter)
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(AlbumTitleRules.MaxLength)
            .IsRequired();

        builder.Property(x => x.ReleaseDate)
            .HasColumnName("release_date");
        builder.HasIndex(x => x.ReleaseDate);

        builder.Property(x => x.IsPublished)
            .HasColumnName("is_published")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion(CatalogEfCoreValueConverters.AlbumTypeConverter)
            .HasColumnName("type");

        builder.OwnsOne(x => x.Cover, coverBuilder =>
        {
            coverBuilder.Property(x => x.ImageId)
                .HasColumnName("cover_image_id")
                .HasConversion(CatalogEfCoreValueConverters.ImageIdConverter)
                .IsRequired();

            coverBuilder.OwnsOne(x => x.Metadata, metadataBuilder =>
            {
                metadataBuilder.Property(x => x.Width)
                    .HasColumnName("cover_metadata_width");

                metadataBuilder.Property(x => x.Height)
                    .HasColumnName("cover_metadata_height");

                metadataBuilder.Property(x => x.FileType)
                    .HasConversion(CatalogEfCoreValueConverters.ImageFileTypeConverter)
                    .HasColumnName("cover_metadata_file_type");

                metadataBuilder.Property(x => x.SizeInBytes)
                    .HasColumnName("cover_metadata_size_in_bytes");
            });
        });

        builder.OwnsMany(t => t.MainArtists, a =>
        {
            a.ToTable("album_main_artists");

            a.Property<AlbumId>("AlbumId")
                .HasColumnName("album_id");

            a.WithOwner().HasForeignKey("AlbumId");

            a.Property(x => x.Value)
                .HasColumnName("artist_id")
                .IsRequired();

            a.HasKey("AlbumId", "Value");
        });

        builder.OwnsMany(t => t.Tracks, a =>
        {
            a.ToTable("album_tracks");

            a.Property<AlbumId>("AlbumId")
                .HasColumnName("album_id");

            a.WithOwner().HasForeignKey("AlbumId");

            a.Property(x => x.Value)
                .HasColumnName("track_id")
                .IsRequired();

            a.HasKey("AlbumId", "Value");
        });

        builder.Ignore(x => x.DomainEvents);
    }
}
