using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Artists.Queries;

public sealed record ArtistDetailsResponse(
    string Name,
    string? Bio,
    string Status,
    ImageMetadataDetailsResult? Avatar,
    ImageMetadataDetailsResult? Banner,
    IEnumerable<ImageMetadataDetailsResult> Gallery);
