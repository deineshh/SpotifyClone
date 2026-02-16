using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Catalog.Application.Errors;

public static class AlbumErrors
{
    public static readonly Error InvalidCoverImage = new(
        "Album.InvalidCoverImage",
        "The specified cover image is invalid.");

    public static readonly Error InvalidTitle = new(
        "Album.InvalidTitle",
        "The specified title is invalid.");

    public static readonly Error InvalidType = new(
        "Album.InvalidType",
        "The specified type is invalid.");

    public static readonly Error InvalidStatus = new(
        "Album.InvalidStatus",
        "The specified status is invalid.");

    public static readonly Error InvalidMainArtists = new(
        "Album.InvalidMainArtists",
        "The specified main artists are invalid.");

    public static readonly Error InvalidTracks = new(
        "Album.InvalidTracks",
        "The specified tracks are invalid.");

    public static readonly Error AlreadyHaveACover = new(
        "Album.AlreadyHaveACover",
        "The album is already attached to a cover.");

    public static readonly Error AlreadyPublished = new(
        "Album.AlreadyPublished",
        "The album has already been published.");

    public static readonly Error CannotPublish = new(
        "Album.CannotPublish",
        "The album cannot be published.");

    public static readonly Error NotFound = CommonErrors.NotFound(
        "Album", "Album");
}
