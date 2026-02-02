using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ArtistEfCoreRepository(CatalogAppDbContext context)
    : IArtistRepository
{
    private readonly DbSet<Artist> _artists = context.Artists;

    public async Task AddAsync(
        Artist artist,
        CancellationToken cancellationToken = default)
        => await _artists.AddAsync(artist, cancellationToken);

    public async Task<Artist?> GetByIdAsync(
        ArtistId id,
        CancellationToken cancellationToken = default)
        => await _artists.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
