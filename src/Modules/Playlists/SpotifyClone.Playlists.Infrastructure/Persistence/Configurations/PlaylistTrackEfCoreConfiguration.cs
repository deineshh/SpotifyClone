using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.Entities;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;
using SpotifyClone.Playlists.Infrastructure.Persistence.Configurations.Converters;

namespace SpotifyClone.Playlists.Infrastructure.Persistence.Configurations;

internal sealed class PlaylistTrackEfCoreConfiguration : IEntityTypeConfiguration<PlaylistTrack>
{
    public void Configure(EntityTypeBuilder<PlaylistTrack> builder)
    {
        builder.ToTable("playlist_tracks");

        builder.Property<PlaylistId>("playlist_id")
            .HasConversion(PlaylistsEfCoreValueConverters.PlaylistIdConverter);

        builder.Property(x => x.Id)
            .HasColumnName("track_id")
            .HasConversion(PlaylistsEfCoreValueConverters.TrackIdConverter)
            .IsRequired();

        builder.HasKey("playlist_id", nameof(PlaylistTrack.Id));

        builder.Property(x => x.Position)
            .HasColumnName("position")
            .IsRequired();
        builder.HasIndex("playlist_id", nameof(PlaylistTrack.Position))
            .IsUnique();

        builder.HasOne<Playlist>()
            .WithMany("_tracks")
            .HasForeignKey("playlist_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
