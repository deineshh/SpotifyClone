using SpotifyClone.Catalog.Application.Models;

namespace SpotifyClone.Catalog.Application.Features.Moods.Queries;

public sealed record MoodDetailsResponse(
    string Name,
    ImageMetadataDetailsResult Cover);
