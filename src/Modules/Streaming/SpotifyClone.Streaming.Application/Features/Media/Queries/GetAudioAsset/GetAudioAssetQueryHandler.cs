using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Abstractions.Services;

namespace SpotifyClone.Streaming.Application.Features.Media.Queries.GetAudioAsset;

internal sealed class GetAudioAssetQueryHandler(
    IFileStorage storage)
    : IQueryHandler<GetAudioAssetQuery, GetAudioAssetQueryResult>
{
    private readonly IFileStorage _storage = storage;

    public async Task<Result<GetAudioAssetQueryResult>> Handle(
        GetAudioAssetQuery request,
        CancellationToken cancellationToken)
    {
        string baseUrl = _storage.GetAudioRootPath();
        string hlsUrl = $"{baseUrl}/{request.AudioId}/master.m3u8";
        string dashUrl = $"{baseUrl}/{request.AudioId}/manifest.mpd";

        return new GetAudioAssetQueryResult(request.AudioId, hlsUrl, dashUrl);
    }
}
