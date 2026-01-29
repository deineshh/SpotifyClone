using Microsoft.EntityFrameworkCore;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Database;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Database;

public sealed class StreamingAppDbContext(DbContextOptions<StreamingAppDbContext> options)
    : ApplicationDbContext<StreamingAppDbContext>("streaming", options)
{
    public DbSet<AudioAsset> AudioAssets => Set<AudioAsset>();
    public DbSet<ImageAsset> ImageAssets => Set<ImageAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StreamingAppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
