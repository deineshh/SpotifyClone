using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Genres.Queries;

public sealed record GenreDetailsResponse(
    string Name,
    ImageMetadataDetailsResult Cover);
