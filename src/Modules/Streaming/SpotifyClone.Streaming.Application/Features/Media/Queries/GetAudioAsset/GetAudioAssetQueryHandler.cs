using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Configuration;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Streaming.Application.Features.Media.Queries.GetAudioAsset;

internal sealed class GetAudioAssetQueryHandler(
    ApplicationSettings appSettings)
    : IQueryHandler<GetAudioAssetQuery, GetAudioAssetQueryResult>
{
    private readonly ApplicationSettings _appSettings = appSettings;

    public async Task<Result<GetAudioAssetQueryResult>> Handle(
        GetAudioAssetQuery request,
        CancellationToken cancellationToken)
    {
        string hlsUrl = $"{_appSettings.ApiUrl}/audio/{request.AudioId}/master.m3u8";
        string dashUrl = $"{_appSettings.ApiUrl}/audio/{request.AudioId}/manifest.mpd";

        return new GetAudioAssetQueryResult(
            request.AudioId,
            hlsUrl,
            dashUrl);
    }
}
