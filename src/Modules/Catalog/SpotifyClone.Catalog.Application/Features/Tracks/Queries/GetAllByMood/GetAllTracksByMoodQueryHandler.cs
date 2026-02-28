using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByMood;

internal sealed class GetAllTracksByMoodQueryHandler(
    IMoodReadService moodReadService,
    ITrackReadService trackReadService)
    : IQueryHandler<GetAllTracksByMoodQuery, TrackList>
{
    private readonly IMoodReadService _moodReadService = moodReadService;
    private readonly ITrackReadService _trackReadService = trackReadService;

    public async Task<Result<TrackList>> Handle(
        GetAllTracksByMoodQuery request,
        CancellationToken cancellationToken)
    {
        var moodId = MoodId.From(request.MoodId);

        bool moodExists = await _moodReadService.ExistsAsync(
            moodId, cancellationToken);
        if (!moodExists)
        {
            return Result.Failure<TrackList>(MoodErrors.NotFound);
        }

        IEnumerable<TrackSummary> tracks = await _trackReadService.GetAllByMoodIdAsync(
            moodId, cancellationToken);

        return new TrackList(tracks.ToList().AsReadOnly());
    }
}
