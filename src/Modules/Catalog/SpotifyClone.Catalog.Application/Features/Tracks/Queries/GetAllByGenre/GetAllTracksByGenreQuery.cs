using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByGenre;

public sealed record GetAllTracksByGenreQuery(
    Guid GenreId)
    : IQuery<TrackList>;
