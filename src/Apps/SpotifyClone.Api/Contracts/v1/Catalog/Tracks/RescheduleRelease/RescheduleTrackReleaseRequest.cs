namespace SpotifyClone.Api.Contracts.v1.Catalog.Tracks.RescheduleRelease;

public sealed record RescheduleTrackReleaseRequest
{
    public required DateTimeOffset ReleaseDate { get; init; } // 2027-04-25T13:45:30
}
