using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Artists.Queries;

public sealed record ArtistDetailsResult(
    string Name,
    string? Bio,
    bool IsVerified,
    ImageMetadataDetailsResult Avatar,
    ImageMetadataDetailsResult Banner,
    IEnumerable<ImageMetadataDetailsResult> Gallery);
