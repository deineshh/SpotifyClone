namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries;

public sealed record TrackSummaryResult(
    string Title,
    TimeSpan? Duration,
    DateTimeOffset? ReleaseDate,
    bool ContainsExplicitContent,
    int TrackNumber,
    string Status,
    Guid? AudioFileId,
    Guid AlbumId);
