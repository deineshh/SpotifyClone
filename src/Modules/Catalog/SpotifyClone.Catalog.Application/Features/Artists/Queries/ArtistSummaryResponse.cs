using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Artists.Queries;

public sealed record ArtistSummaryResponse(
    string Name,
    string Status,
    ImageMetadataDetailsResult? Avatar);
