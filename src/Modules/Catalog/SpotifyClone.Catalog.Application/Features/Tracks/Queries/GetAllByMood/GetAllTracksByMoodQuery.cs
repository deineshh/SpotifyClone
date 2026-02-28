using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByMood;

public sealed record GetAllTracksByMoodQuery(
    Guid MoodId)
    : IQuery<TrackList>;
