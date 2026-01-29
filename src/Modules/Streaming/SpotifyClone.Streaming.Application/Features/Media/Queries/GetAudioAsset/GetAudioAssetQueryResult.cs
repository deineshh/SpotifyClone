namespace SpotifyClone.Streaming.Application.Features.Media.Queries.GetAudioAsset;

public sealed record GetAudioAssetQueryResult(
    Guid AudioId,
    string HlsUrl,
    string DashUrl);
