using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Genres.Queries;

public sealed record GenreDetailsResult(
    string Name,
    ImageMetadataDetailsResult Cover);
