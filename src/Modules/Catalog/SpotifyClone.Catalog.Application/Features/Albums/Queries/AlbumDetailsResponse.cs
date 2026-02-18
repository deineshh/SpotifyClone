using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Albums.Queries;

public sealed record AlbumDetailsResponse(
    string Title,
    DateTimeOffset? ReleaseDate,
    string Status,
    string Type,
    ImageMetadataDetailsResult? Cover,
    IEnumerable<ArtistSummaryResponse> MainArtists,
    IEnumerable<TrackSummaryResponse> Tracks);
