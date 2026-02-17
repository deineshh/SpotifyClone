using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Albums.Entities;

public sealed class AlbumTrack
{
    public TrackId TrackId { get; private set; } = null!;

    private AlbumTrack()
    {
    }

    internal AlbumTrack(TrackId trackId)
        => TrackId = trackId;
}
