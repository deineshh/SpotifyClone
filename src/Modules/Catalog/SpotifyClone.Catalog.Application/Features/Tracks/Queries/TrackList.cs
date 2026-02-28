namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries;

public sealed record TrackList(
    IReadOnlyCollection<TrackSummary> Tracks);
