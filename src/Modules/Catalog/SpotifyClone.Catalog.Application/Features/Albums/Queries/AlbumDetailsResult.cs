using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Albums.Queries;

public sealed record AlbumDetailsResult(
    string Title,
    DateTimeOffset? ReleaseDate,
    string Status,
    string? Type,
    ImageMetadataDetailsResult Cover,
    IEnumerable<ArtistSummaryResult> MainArtists,
    IEnumerable<TrackSummaryResult> Tracks);
