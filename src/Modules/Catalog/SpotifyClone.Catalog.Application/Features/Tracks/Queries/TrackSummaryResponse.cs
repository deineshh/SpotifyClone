namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries;

public sealed record TrackSummaryResponse(
    string Title,
    TimeSpan? Duration,
    DateTimeOffset? ReleaseDate,
    bool ContainsExplicitContent,
    int TrackNumber,
    string Status,
    Guid? AudioFileId,
    Guid? AlbumId);
