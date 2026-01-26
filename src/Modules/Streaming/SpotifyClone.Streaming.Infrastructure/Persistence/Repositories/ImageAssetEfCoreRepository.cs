using Microsoft.EntityFrameworkCore;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;
using SpotifyClone.Streaming.Infrastructure.Persistence.Database;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Repositories;

internal sealed class ImageAssetEfCoreRepository(
    StreamingAppDbContext context)
    : IImageAssetRepository
{
    private readonly DbSet<ImageAsset> _imageAssets = context.ImageAssets;

    public async Task AddAsync(
        ImageAsset imageAsset,
        CancellationToken cancellationToken = default)
        => await _imageAssets.AddAsync(imageAsset, cancellationToken);

    public async Task<ImageAsset?> GetByIdAsync(
        ImageId id,
        CancellationToken cancellationToken = default)
        => await _imageAssets.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
