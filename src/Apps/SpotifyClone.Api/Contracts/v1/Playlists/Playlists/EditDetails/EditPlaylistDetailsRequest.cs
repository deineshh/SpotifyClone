namespace SpotifyClone.Api.Contracts.v1.Playlists.Playlists.EditDetails;

public sealed record EditPlaylistDetailsRequest
{
    public required Guid PlaylistId { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required bool IsPublic { get; init; }
}
