using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;

public interface IAudioAssetRepository
{
    Task<AudioAsset?> GetByUserIdAsync(
        AudioAssetId audioAssetId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AudioAsset audioAsset,
        CancellationToken cancellationToken = default);
}
