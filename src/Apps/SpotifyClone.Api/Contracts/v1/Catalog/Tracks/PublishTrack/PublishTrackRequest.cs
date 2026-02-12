namespace SpotifyClone.Api.Contracts.v1.Catalog.Tracks.PublishTrack;

public sealed record PublishTrackRequest
{
    public required DateTimeOffset ReleaseDate { get; init; } // 2026-04-25T13:45:30
}
