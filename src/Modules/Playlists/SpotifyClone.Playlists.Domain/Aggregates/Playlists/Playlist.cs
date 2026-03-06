using SpotifyClone.Playlists.Domain.Aggregates.Playlists.Entities;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.Events;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.Exceptions;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.Rules;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Playlists.Domain.Aggregates.Playlists;

public sealed class Playlist : AggregateRoot<PlaylistId, Guid>
{
    private const int PositionStep = 1000;

    private readonly HashSet<UserId> _collaborators = [];
    private readonly HashSet<PlaylistTrack> _tracks = [];

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public UserId OwnerId { get; private set; } = null!;
    public PlaylistCoverImage? Cover { get; private set; }
    public bool IsPublic { get; private set; }
    public IReadOnlySet<UserId> Collaborators => _collaborators;
    public IReadOnlySet<PlaylistTrack> Tracks => _tracks;

    public static Playlist Create(
        PlaylistId id,
        string name,
        string? description,
        UserId ownerId,
        PlaylistCoverImage? cover,
        bool isPublic)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(ownerId);

        PlaylistNameRules.Validate(name);
        PlaylistDescriptionRules.Validate(description);

        return new Playlist(id, name, description, ownerId, cover, isPublic);
    }

    public void Rename(string name)
    {
        PlaylistNameRules.Validate(name);
        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        PlaylistDescriptionRules.Validate(description);
        Description = description;
    }

    public void LinkNewCover(PlaylistCoverImage cover)
    {
        ArgumentNullException.ThrowIfNull(cover);

        TryUnlinkCover();

        Cover = cover;
        RaiseDomainEvent(new PlaylistLinkedToCoverImageDomainEvent(Cover.ImageId));
    }

    public void TryUnlinkCover()
    {
        if (Cover is null)
        {
            return;
        }

        RaiseDomainEvent(new PlaylistUnlinkedFromCoverImageDomainEvent(Cover.ImageId));
        Cover = null;
    }

    public void MakePublic() => IsPublic = true;

    public void MakePrivate() => IsPublic = false;

    public void AddCollaborator(UserId collaboratorId)
    {
        if (_collaborators.Contains(collaboratorId))
        {
            return;
        }

        if (_collaborators.Count >= 1000)
        {
            throw new InvalidPlaylistCollaboratorsDomainException(
                "Cannot add more than 1000 collaborators to a playlist.");
        }

        if (_collaborators.Add(collaboratorId))
        {
            // Domain event can be raised here if needed
        }
    }

    public void RemoveCollaborator(UserId collaboratorId)
    {
        if (_collaborators.Remove(collaboratorId))
        {
            // Domain event can be raised here if needed
        }
    }

    public void AddTrack(TrackId trackId)
    {
        ArgumentNullException.ThrowIfNull(trackId);

        int nextPosition = _tracks.Count != 0
            ? _tracks.Max(t => t.Position) + PositionStep
            : PositionStep;

        var track = new PlaylistTrack(trackId, nextPosition);

        if (_tracks.Add(track))
        {
            // Domain event can be raised here
        }
    }

    public void RemoveTrack(TrackId trackId)
    {
        ArgumentNullException.ThrowIfNull(trackId);

        PlaylistTrack? track = _tracks.FirstOrDefault(t => t.Id == trackId);
        if (track is null || !_tracks.Remove(track!))
        {
            throw new TrackNotFoundInPlaylistDomainException(
                $"Cannot remove track '{trackId.Value}' from the playlist, " +
                $"because it was not found in the playlist.");
        }

        // Domain event can be raised here
    }

    public void MoveTrack(TrackId trackId, int targetIndex)
    {
        PlaylistTrack trackToMove = _tracks.SingleOrDefault(t => t.Id == trackId)
            ?? throw new TrackNotFoundInPlaylistDomainException(trackId.Value.ToString());

        // 1. Get current sorted list to identify neighbors
        var sortedTracks = _tracks.OrderBy(t => t.Position).ToList();
        sortedTracks.Remove(trackToMove); // Remove it from current spot to simulate the "gap"

        // Clamp the index to ensure it's within bounds
        targetIndex = Math.Clamp(targetIndex, 0, sortedTracks.Count);

        int newPosition;

        // 2. Calculate Midpoint
        if (targetIndex == 0)
        {
            // Moving to the very top
            int firstPos = sortedTracks.Count > 0 ? sortedTracks[0].Position : PositionStep;
            newPosition = firstPos / 2;
        }
        else if (targetIndex >= sortedTracks.Count)
        {
            // Moving to the very bottom
            int lastPos = sortedTracks[^1].Position;
            newPosition = lastPos + PositionStep;
        }
        else
        {
            // Moving between two tracks
            int prevPos = sortedTracks[targetIndex - 1].Position;
            int nextPos = sortedTracks[targetIndex].Position;
            newPosition = (prevPos + nextPos) / 2;

            // 3. Collision Detection (The "Rebalance" Trigger)
            if (newPosition == prevPos || newPosition == nextPos)
            {
                RebalanceTrackPositions();
                // Recurse once after rebalancing to find the new midpoint in a fresh 1000-step list
                MoveTrack(trackId, targetIndex);
                return;
            }
        }

        trackToMove.ChangePosition(newPosition);
    }

    private void RebalanceTrackPositions()
    {
        var sorted = _tracks.OrderBy(t => t.Position).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].ChangePosition((i + 1) * PositionStep);
        }
    }

    private Playlist(
        PlaylistId id,
        string name,
        string? description,
        UserId ownerId,
        PlaylistCoverImage? cover,
        bool isPublic)
        : base(id)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        Cover = cover;
        IsPublic = isPublic;
    }

    private Playlist()
    {
    }
}
