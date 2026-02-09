using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Moods.Queries;

public sealed record MoodDetailsResult(
    string Name,
    ImageMetadataDetailsResult Cover);
