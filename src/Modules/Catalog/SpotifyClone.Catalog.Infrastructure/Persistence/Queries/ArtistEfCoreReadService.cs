using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Models;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Queries;

internal sealed class ArtistEfCoreReadService(
    CatalogAppDbContext context)
    : IArtistReadService
{
    private readonly CatalogAppDbContext _context = context;

    public async Task<ArtistDetailsResponse?> GetDetailsAsync(
        ArtistId id,
        CancellationToken cancellationToken = default)
        => await _context.Artists
        .Where(a => a.Id == id)
        .Select(a => new ArtistDetailsResponse(
            a.Name,
            a.Bio,
            a.Status.Value,
            a.Avatar == null ? null : new ImageMetadataDetailsResult(
                a.Avatar.ImageId.Value,
                a.Avatar.Metadata.Width,
                a.Avatar.Metadata.Height,
                a.Avatar.Metadata.FileType.Value,
                a.Avatar.Metadata.SizeInBytes),
            a.Banner == null ? null : new ImageMetadataDetailsResult(
                a.Banner.ImageId.Value,
                a.Banner.Metadata.Width,
                a.Banner.Metadata.Height,
                a.Banner.Metadata.FileType.Value,
                a.Banner.Metadata.SizeInBytes),
            a.Gallery.Select(img => new ImageMetadataDetailsResult(
                img.ImageId!.Value,
                img.Metadata.Width,
                img.Metadata.Height,
                img.Metadata.FileType.Value,
                img.Metadata.SizeInBytes
            )).ToList()))
        .SingleOrDefaultAsync(cancellationToken);
}
