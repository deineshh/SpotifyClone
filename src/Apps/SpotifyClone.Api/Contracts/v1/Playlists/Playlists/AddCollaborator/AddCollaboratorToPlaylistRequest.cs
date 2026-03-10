namespace SpotifyClone.Api.Contracts.v1.Playlists.Playlists.AddCollaborator;

public sealed record AddCollaboratorToPlaylistRequest
{
    public required Guid CollaboratorId { get; init; }
}
