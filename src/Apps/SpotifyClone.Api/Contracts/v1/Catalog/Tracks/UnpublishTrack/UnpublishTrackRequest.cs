namespace SpotifyClone.Api.Contracts.v1.Catalog.Tracks.UnpublishTrack;

public sealed record UnpublishTrackRequest
{
    public required Guid TrackId { get; init; }
}
