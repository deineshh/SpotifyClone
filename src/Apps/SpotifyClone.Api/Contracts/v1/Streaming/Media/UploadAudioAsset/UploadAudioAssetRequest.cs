namespace SpotifyClone.Api.Contracts.v1.Streaming.Media.UploadAudioAsset;

public sealed class UploadAudioAssetRequest
{
    public required string Title { get; set; }
    public required string Artist { get; set; }
    public required IFormFile File { get; set; }
}
