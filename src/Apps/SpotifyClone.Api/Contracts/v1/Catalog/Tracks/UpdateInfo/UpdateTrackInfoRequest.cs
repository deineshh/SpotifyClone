namespace SpotifyClone.Api.Contracts.v1.Catalog.Tracks.UpdateInfo;

public sealed record UpdateTrackInfoRequest
{
    public required string Title { get; init; }
    public required DateTimeOffset ReleaseDate { get; init; }
    public required bool ContainsExplicitContent { get; init; }
    public required int TrackNumber { get; init; }
}
