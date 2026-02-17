using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Entities;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class AlbumTrackEfCoreConfiguration : IEntityTypeConfiguration<AlbumTrack>
{
    public void Configure(EntityTypeBuilder<AlbumTrack> builder)
    {
        builder.ToTable("album_tracks");

        builder.Property<AlbumId>("album_id")
            .HasConversion(CatalogEfCoreValueConverters.AlbumIdConverter);

        builder.HasKey("album_id", nameof(AlbumTrack.TrackId));

        builder.Property(x => x.TrackId)
            .HasColumnName("track_id")
            .HasConversion(CatalogEfCoreValueConverters.TrackIdConverter)
            .IsRequired();

        builder.HasOne<Album>()
            .WithMany("_tracks")
            .HasForeignKey("album_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
