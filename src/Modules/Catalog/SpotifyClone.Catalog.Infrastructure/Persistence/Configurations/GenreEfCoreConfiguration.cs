using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Catalog.Domain.Aggregates.Genres;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.Rules;
using SpotifyClone.Catalog.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class GenreEfCoreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id")
            .HasConversion(CatalogEfCoreValueConverters.GenreIdConverter)
            .ValueGeneratedNever();

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .HasMaxLength(GenreNameRules.MaxLength)
            .IsRequired();

        builder.OwnsOne(g => g.Cover, coverBuilder =>
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

        builder.Ignore(g => g.DomainEvents);
    }
}
