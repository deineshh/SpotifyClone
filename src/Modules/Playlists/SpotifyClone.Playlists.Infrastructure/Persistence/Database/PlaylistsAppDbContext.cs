using Microsoft.EntityFrameworkCore;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Database;

namespace SpotifyClone.Playlists.Infrastructure.Persistence.Database;

public sealed class PlaylistsAppDbContext(DbContextOptions<PlaylistsAppDbContext> options)
    : ApplicationDbContext<PlaylistsAppDbContext>("playlists", options)
{
    public DbSet<Playlist> Playlists => Set<Playlist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlaylistsAppDbContext).Assembly);
    }
}
