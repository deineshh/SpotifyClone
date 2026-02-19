using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;

public sealed record AlbumTrack : ValueObject
{
    public TrackId TrackId { get; private set; } = null!;

    private AlbumTrack()
    {
    }

    internal AlbumTrack(TrackId trackId)
        => TrackId = trackId;
}
