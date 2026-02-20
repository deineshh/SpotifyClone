using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Genres.Queries;
using SpotifyClone.Catalog.Application.Features.Moods.Queries;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries;

public sealed record TrackDetailsResponse(
    string Title,
    TimeSpan? Duration,
    DateTimeOffset? ReleaseDate,
    bool ContainsExplicitContent,
    string Status,
    Guid? AudioFileId,
    Guid? AlbumId,
    IEnumerable<ArtistSummaryResponse> MainArtists,
    IEnumerable<ArtistSummaryResponse> FeaturedArtists,
    IEnumerable<GenreSummaryResponse> Genres,
    IEnumerable<MoodSummaryResponse> Moods);
