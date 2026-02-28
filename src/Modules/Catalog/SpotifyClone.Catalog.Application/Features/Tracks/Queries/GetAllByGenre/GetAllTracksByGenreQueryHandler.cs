using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByGenre;

internal sealed class GetAllTracksByGenreQueryHandler(
    IGenreReadService genreReadService,
    ITrackReadService trackReadService)
    : IQueryHandler<GetAllTracksByGenreQuery, TrackList>
{
    private readonly IGenreReadService _genreReadService = genreReadService;
    private readonly ITrackReadService _trackReadService = trackReadService;

    public async Task<Result<TrackList>> Handle(
        GetAllTracksByGenreQuery request,
        CancellationToken cancellationToken)
    {
        var genreId = GenreId.From(request.GenreId);

        bool genreExists = await _genreReadService.ExistsAsync(
            genreId, cancellationToken);
        if (!genreExists)
        {
            return Result.Failure<TrackList>(GenreErrors.NotFound);
        }

        IEnumerable<TrackSummary> tracks = await _trackReadService.GetAllByGenreIdAsync(
            genreId, cancellationToken);

        return new TrackList(tracks.ToList().AsReadOnly());
    }
}
