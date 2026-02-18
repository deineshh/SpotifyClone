using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Albums.Queries;

public sealed record AlbumSummaryResponse(
    string Title,
    DateTimeOffset? ReleaseDate,
    string Status,
    string Type,
    ImageMetadataDetailsResult Cover);
