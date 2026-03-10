using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Playlists.Domain.Aggregates.Playlists.Entities;

public sealed class PlaylistTrack : Entity<TrackId, Guid>
{
    public UserId CreatorId { get; private set; } = null!;
    public int Position { get; private set; }

    internal PlaylistTrack(TrackId trackId, UserId creatorId, int position)
        : base(trackId)
    {
        CreatorId = creatorId;
        Position = position;
    }

    internal void ChangePosition(int newPosition)
    {
        if (newPosition == Position)
        {
            return;
        }

        Position = newPosition;
    }

    private PlaylistTrack()
    {
    }
}
