using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Playlists.Application.Errors;

public static class PlaylistErrors
{
    public static readonly Error InvalidName = new(
        "Playlist.InvalidName",
        "Playlist name is invalid.");

    public static readonly Error InvalidDescription = new(
        "Playlist.InvalidDescription",
        "Playlist description is invalid.");

    public static readonly Error InvalidCover = new(
        "Playlist.InvalidCover",
        "Playlist cover is invalid.");

    public static readonly Error InvalidCollaborators = new(
        "Playlist.InvalidCollaborators",
        "Playlist collaborators are invalid.");

    public static readonly Error TrackNotFound = new(
        "Playlist.TrackNotFound",
        "Track not found in the playlist.");

    public static readonly Error NotFound = CommonErrors.NotFound(
        "Playlist", "Playlist");
}
