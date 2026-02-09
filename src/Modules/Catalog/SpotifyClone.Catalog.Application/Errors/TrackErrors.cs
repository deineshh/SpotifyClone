using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Catalog.Application.Errors;

public static class TrackErrors
{
    public static readonly Error InvalidTitle = new(
        "Track.InvalidTitle",
        "The specified title is invalid.");

    public static readonly Error InvalidDuration = new(
        "Track.InvalidDuration",
        "The specified duration is invalid.");

    public static readonly Error InvalidMainArtists = new(
        "Track.InvalidMainArtists",
        "The specified main artists are invalid.");

    public static readonly Error InvalidGenres = new(
        "Track.InvalidGenres",
        "The specified genres are invalid.");

    public static readonly Error InvalidMoods = new(
        "Track.InvalidMoods",
        "The specified moods are invalid.");

    public static readonly Error InvalidReleaseDate = new(
        "Track.InvalidReleaseDate",
        "The specified release date is invalid.");

    public static readonly Error InvalidTrackStatus = new(
        "Track.InvalidTrackStatus",
        "Track status is invalid.");

    public static readonly Error AlreadyPublished = new(
        "Track.AlreadyPublished",
        "The track has already been published.");

    public static readonly Error AlreadyLinkedToAudioFile = new(
        "Track.AlreadyLinkedToAudio",
        "Track is already linked to an audio.");

    public static readonly Error CannotPublish = new(
        "Track.CannotPublish",
        "The specified track cannot be published.");

    public static readonly Error NotPublished = new(
        "Track.NotPublished",
        "The specified track is not published.");

    public static readonly Error NotFound = CommonErrors.NotFound(
        "Track", "Track");
}
