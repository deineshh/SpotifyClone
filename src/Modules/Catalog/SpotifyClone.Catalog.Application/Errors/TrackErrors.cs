using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Catalog.Application.Errors;

public static class TrackErrors
{
    public static readonly Error InvalidDuration = new(
        "Track.InvalidDuration",
        "The specified duration is invalid.");

    public static readonly Error InvalidGenres = new(
        "Track.InvalidGenres",
        "The specified genres are invalid.");

    public static readonly Error InvalidMainArtists = new(
        "Track.InvalidMainArtists",
        "The specified main artists are invalid.");

    public static readonly Error InvalidTitle = new(
        "Track.InvalidTitle",
        "The specified title are invalid.");

    public static readonly Error AlreadyPublished = new(
        "Track.AlreadyPublished",
        "The track has already been published.");
}
