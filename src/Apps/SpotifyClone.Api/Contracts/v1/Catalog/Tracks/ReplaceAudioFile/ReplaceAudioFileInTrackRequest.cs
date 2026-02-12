namespace SpotifyClone.Api.Contracts.v1.Catalog.Tracks.ReplaceAudioFile;

public sealed record ReplaceAudioFileInTrackRequest
{
    public required Guid AudioFileId { get; init; }
}
