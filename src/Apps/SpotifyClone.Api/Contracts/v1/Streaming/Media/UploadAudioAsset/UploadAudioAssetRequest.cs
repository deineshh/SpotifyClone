namespace SpotifyClone.Api.Contracts.v1.Streaming.Media.UploadAudioAsset;

public sealed class UploadAudioAssetRequest
{
    public required IFormFile File { get; set; }
}
