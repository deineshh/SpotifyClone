using Microsoft.EntityFrameworkCore;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;
using SpotifyClone.Streaming.Infrastructure.Persistence.Database;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Repositories;

internal sealed class AudioAssetEfCoreRepository(
    StreamingAppDbContext context)
    : IAudioAssetRepository
{
    private readonly DbSet<AudioAsset> _audioAssets = context.AudioAssets;

    public async Task AddAsync(
        AudioAsset audioAsset,
        CancellationToken cancellationToken = default)
        => await _audioAssets.AddAsync(audioAsset, cancellationToken);

    public async Task<AudioAsset?> GetByUserIdAsync(
        AudioAssetId audioAssetId,
        CancellationToken cancellationToken = default)
        => await _audioAssets.FirstOrDefaultAsync(a => a.Id == audioAssetId, cancellationToken);
}
