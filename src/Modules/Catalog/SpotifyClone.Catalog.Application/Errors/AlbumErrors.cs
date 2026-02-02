using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Catalog.Application.Errors;

public static class AlbumErrors
{
    public static readonly Error InvalidCoverImage = new(
        "Album.InvalidCoverImage",
        "The specified cover image is invalid.");

    public static readonly Error InvalidMainArtists = new(
        "Album.InvalidMainArtists",
        "The specified main artists are invalid.");

    public static readonly Error InvalidTitle = new(
        "Album.InvalidTitle",
        "The specified title is invalid.");

    public static readonly Error InvalidTracks = new(
        "Album.InvalidTracks",
        "The specified tracks are invalid.");

    public static readonly Error InvalidType = new(
        "Album.InvalidType",
        "The specified type is invalid.");

    public static readonly Error AlreadyPublished = new(
        "Album.AlreadyPublished",
        "The album has already been published.");
}
