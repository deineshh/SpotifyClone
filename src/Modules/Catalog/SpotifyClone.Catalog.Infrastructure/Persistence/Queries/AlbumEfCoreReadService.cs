using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Features.Albums.Queries;
using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Models;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Queries;

internal sealed class AlbumEfCoreReadService(
    CatalogAppDbContext context)
    : IAlbumReadService
{
    private readonly CatalogAppDbContext _context = context;

    public async Task<AlbumDetailsResponse?> GetDetailsAsync(
        AlbumId id,
        CancellationToken cancellationToken = default)
        => await _context.Albums
        .Where(a => a.Id == id)
        .Select(a => new AlbumDetailsResponse(
            a.Title,
            a.ReleaseDate,
            a.Status.Value,
            a.Type.Value,
            a.Cover == null ? null : new ImageMetadataDetailsResult(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes),
            _context.Artists
                .Where(artist => a.MainArtists.Contains(artist.Id))
                .Select(artist => new ArtistSummaryResponse(
                    artist.Name,
                    artist.IsVerified,
                    artist.Avatar == null ? null : new ImageMetadataDetailsResult(
                        artist.Avatar.ImageId.Value,
                        artist.Avatar.Metadata.Width,
                        artist.Avatar.Metadata.Height,
                        artist.Avatar.Metadata.FileType.Value,
                        artist.Avatar.Metadata.SizeInBytes))
                ).ToList(),
            _context.Tracks
                .Where(t => a.Tracks.Contains(t.Id))
                .Select(t => new TrackSummaryResponse(
                    t.Title,
                    t.Duration,
                    t.ReleaseDate,
                    t.ContainsExplicitContent,
                    t.Status.Value,
                    t.AudioFileId == null ? null : t.AudioFileId.Value,
                    t.AlbumId == null ? null : t.AlbumId.Value))
                .ToList()))
        .SingleOrDefaultAsync(cancellationToken);
}
