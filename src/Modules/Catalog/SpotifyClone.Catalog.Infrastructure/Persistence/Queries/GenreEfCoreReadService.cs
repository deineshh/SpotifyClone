using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Features.Genres.Queries;
using SpotifyClone.Catalog.Application.Models;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Queries;

internal sealed class GenreEfCoreReadService(
    CatalogAppDbContext context)
    : IGenreReadService
{
    private readonly CatalogAppDbContext _context = context;

    public async Task<GenreDetailsResponse?> GetDetailsAsync(
        GenreId id,
        CancellationToken cancellationToken = default)
        => await _context.Genres
        .Where(g => g.Id == id)
        .Select(g => new GenreDetailsResponse(
            g.Name,
            new ImageMetadataDetailsResult(
                g.Cover.ImageId.Value,
                g.Cover.Metadata.Width,
                g.Cover.Metadata.Height,
                g.Cover.Metadata.FileType.Value,
                g.Cover.Metadata.SizeInBytes)))
        .SingleOrDefaultAsync(cancellationToken);
}
