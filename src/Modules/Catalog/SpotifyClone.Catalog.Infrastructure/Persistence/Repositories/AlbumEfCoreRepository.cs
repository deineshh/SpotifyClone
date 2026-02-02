using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class AlbumEfCoreRepository(CatalogAppDbContext context)
    : IAlbumRepository
{
    private readonly DbSet<Album> _albums = context.Albums;

    public async Task AddAsync(
        Album album,
        CancellationToken cancellationToken = default)
        => await _albums.AddAsync(album, cancellationToken);

    public async Task<Album?> GetByIdAsync(
        AlbumId id,
        CancellationToken cancellationToken = default)
        => await _albums.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
