using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Features.Albums.Queries;
using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Models;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Enums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Queries;

internal sealed class AlbumEfCoreReadService(
    CatalogAppDbContext context)
    : IAlbumReadService
{
    private readonly CatalogAppDbContext _context = context;

    public async Task<AlbumDetails?> GetDetailsAsync(
        AlbumId id,
        CancellationToken cancellationToken = default)
        => await _context.Albums
        .Where(a => a.Id == id)
        .Select(a => new AlbumDetails(
            a.Id.Value,
            a.Title,
            a.ReleaseDate,
            a.Status.Value,
            a.Type.Value,
            a.Cover == null ? null : new ImageMetadataDetails(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes),
            _context.Artists
                .Where(artist => a.MainArtists.Contains(artist.Id))
                .Select(artist => new ArtistSummary(
                    artist.Id.Value,
                    artist.Name,
                    artist.Status.Value,
                    artist.Avatar == null ? null : new ImageMetadataDetails(
                        artist.Avatar.ImageId.Value,
                        artist.Avatar.Metadata.Width,
                        artist.Avatar.Metadata.Height,
                        artist.Avatar.Metadata.FileType.Value,
                        artist.Avatar.Metadata.SizeInBytes))
                ).ToList(),
            a.Tracks.Select(t => new AlbumTrackSummary(t.Id.Value, t.Position))))
        .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<AlbumSummary>> GetAllByArtistIdAsync(
        ArtistId artistId,
        CancellationToken cancellationToken = default)
        => await _context.Albums
        .Where(a => a.MainArtists.Any(a => a.Value == artistId.Value))
        .Select(a => new AlbumSummary(
            a.Id.Value,
            a.Title,
            a.ReleaseDate,
            a.Status.Value,
            a.Type.Value,
            a.Cover == null ? null : new ImageMetadataDetails(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes),
            _context.Artists
            .Where(art => a.MainArtists.Select(ma => ma.Value).Contains(art.Id.Value))
            .Select(art => new ArtistSummary(
                art.Id.Value,
                art.Name,
                art.Status.Value,
                art.Avatar == null ? null : new ImageMetadataDetails(
                    art.Avatar.ImageId.Value,
                    art.Avatar.Metadata.Width,
                    art.Avatar.Metadata.Height,
                    art.Avatar.Metadata.FileType.Value,
                    art.Avatar.Metadata.SizeInBytes)))
            .ToList()))
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<AlbumSummary>> GetAllPublishedByArtistIdAsync(
        ArtistId artistId,
        CancellationToken cancellationToken = default)
        => await _context.Albums
        .Where(a =>
            a.Status.Value == AlbumStatus.Published.Value &&
            a.MainArtists.Any(a => a.Value == artistId.Value))
        .Select(a => new AlbumSummary(
            a.Id.Value,
            a.Title,
            a.ReleaseDate,
            a.Status.Value,
            a.Type.Value,
            a.Cover == null ? null : new ImageMetadataDetails(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes),
            _context.Artists
            .Where(art => a.MainArtists.Select(ma => ma.Value).Contains(art.Id.Value))
            .Select(art => new ArtistSummary(
                art.Id.Value,
                art.Name,
                art.Status.Value,
                art.Avatar == null ? null : new ImageMetadataDetails(
                    art.Avatar.ImageId.Value,
                    art.Avatar.Metadata.Width,
                    art.Avatar.Metadata.Height,
                    art.Avatar.Metadata.FileType.Value,
                    art.Avatar.Metadata.SizeInBytes)))
            .ToList()))
        .ToListAsync(cancellationToken);
}
