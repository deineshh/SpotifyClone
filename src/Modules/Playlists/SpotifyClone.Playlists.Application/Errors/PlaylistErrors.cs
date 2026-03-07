using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Playlists.Application.Errors;

public static class PlaylistErrors
{
    public static readonly Error InvalidMetadata = new(
        "Playlist.InvalidMetadata",
        "Playlist metadata is invalid.");

    public static readonly Error InvalidCover = new(
        "Playlist.InvalidCover",
        "Playlist cover is invalid.");

    public static readonly Error InvalidCollaborators = new(
        "Playlist.InvalidCollaborators",
        "Playlist collaborators are invalid.");

    public static readonly Error TrackNotFound = new(
        "Playlist.TrackNotFound",
        "Track not found in the playlist.");

    public static readonly Error InvalidOwner = new(
        "Playlist.InvalidOwner",
        "The provided owner of the playlist if invalid.");

    public static readonly Error NotOwned = new(
        "Playlist.NotOwned",
        "Playlist is not owned by the current user.");

    public static readonly Error NotFound = CommonErrors.NotFound(
        "Playlist", "Playlist");
}
