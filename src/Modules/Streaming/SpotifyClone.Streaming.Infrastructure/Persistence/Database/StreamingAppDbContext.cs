using Microsoft.EntityFrameworkCore;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Database;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.Database;

public sealed class StreamingAppDbContext(DbContextOptions<StreamingAppDbContext> options)
    : ApplicationDbContext<StreamingAppDbContext>("streaming", options)
{
    public DbSet<AudioAsset> AudioAssets => Set<AudioAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StreamingAppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
