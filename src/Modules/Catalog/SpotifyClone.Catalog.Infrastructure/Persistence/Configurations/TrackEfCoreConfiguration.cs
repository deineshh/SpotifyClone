using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Rules;
using SpotifyClone.Catalog.Infrastructure.Persistence.Configurations.Converters;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class TrackEfCoreConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("tracks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(CatalogEfCoreValueConverters.TrackIdConverter)
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(TrackTitleRules.MaxLength)
            .IsRequired();

        builder.Property(x => x.Duration)
            .HasColumnName("duration");

        builder.Property(x => x.ReleaseDate)
            .HasColumnName("release_date");

        builder.Property(x => x.ContainsExplicitContent)
            .HasColumnName("contains_explicit_content")
            .IsRequired();

        builder.Property(x => x.TrackNumber)
            .HasColumnName("track_number")
            .IsRequired();

        builder.Property(x => x.IsPublished)
            .HasColumnName("is_published")
            .IsRequired();

        builder.Property(x => x.AudioFileId)
            .HasColumnName("audio_file_id")
            .HasConversion(CatalogEfCoreValueConverters.AudioFileIdConverter);
        builder.HasIndex(x => x.AudioFileId)
            .IsUnique();

        builder.Property(x => x.AlbumId)
            .HasColumnName("album_id")
            .HasConversion(CatalogEfCoreValueConverters.AlbumIdConverter)
            .IsRequired();
        builder.HasIndex(x => x.AlbumId);

        builder.OwnsMany(t => t.MainArtists, a =>
        {
            a.ToTable("track_main_artists");

            a.Property<TrackId>("TrackId")
                .HasColumnName("track_id");

            a.WithOwner().HasForeignKey("TrackId");

            a.Property(x => x.Value)
                .HasColumnName("artist_id")
                .IsRequired();

            a.HasKey("TrackId", "Value");
        });

        builder.OwnsMany(t => t.FeaturedArtists, a =>
        {
            a.ToTable("track_featured_artists");

            a.Property<TrackId>("TrackId")
                .HasColumnName("track_id");

            a.WithOwner().HasForeignKey("TrackId");

            a.Property(x => x.Value)
                .HasColumnName("artist_id")
                .IsRequired();

            a.HasKey("TrackId", "Value");
        });

        builder.OwnsMany(t => t.Genres, g =>
        {
            g.ToTable("track_genres");

            g.Property<TrackId>("TrackId")
                .HasColumnName("track_id");

            g.WithOwner().HasForeignKey("TrackId");

            g.Property(x => x.Value)
                .HasColumnName("genre_id")
                .IsRequired();

            g.HasKey("TrackId", "Value");
        });

        builder.Ignore(x => x.DomainEvents);
    }
}
