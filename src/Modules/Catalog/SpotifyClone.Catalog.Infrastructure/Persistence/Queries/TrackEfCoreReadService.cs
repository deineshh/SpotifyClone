using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Genres.Queries;
using SpotifyClone.Catalog.Application.Features.Moods.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Models;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Queries;

internal sealed class TrackEfCoreReadService(
    CatalogAppDbContext context)
    : ITrackReadService
{
    private readonly CatalogAppDbContext _context = context;

    public async Task<TrackDetails?> GetDetailsAsync(
        TrackId id,
        CancellationToken cancellationToken = default)
        => await _context.Tracks
        .Where(t => t.Id == id)
        .Select(t => new TrackDetails(
            t.Id.Value,
            t.Title,
            t.Duration,
            t.ReleaseDate,
            t.ContainsExplicitContent,
            t.Status.Value,
            t.AudioFileId == null ? null : t.AudioFileId.Value,
            t.AlbumId == null ? null : t.AlbumId.Value,
            _context.Artists
                .Where(a => t.MainArtists.Contains(a.Id))
                .Select(a => new ArtistSummary(
                    a.Id.Value,
                    a.Name,
                    a.Status.Value,
                    a.Avatar == null ? null : new ImageMetadataDetails(
                        a.Avatar.ImageId.Value,
                        a.Avatar.Metadata.Width,
                        a.Avatar.Metadata.Height,
                        a.Avatar.Metadata.FileType.Value,
                        a.Avatar.Metadata.SizeInBytes))
                ).ToList(),
            _context.Artists
                .Where(a => t.FeaturedArtists.Contains(a.Id))
                .Select(a => new ArtistSummary(
                    a.Id.Value,
                    a.Name,
                    a.Status.Value,
                    a.Avatar == null ? null : new ImageMetadataDetails(
                        a.Avatar.ImageId.Value,
                        a.Avatar.Metadata.Width,
                        a.Avatar.Metadata.Height,
                        a.Avatar.Metadata.FileType.Value,
                        a.Avatar.Metadata.SizeInBytes))
                ).ToList(),
            _context.Genres
                .Where(g => t.Genres.Contains(g.Id))
                .Select(g => new GenreSummary(
                    g.Id.Value,
                    g.Name)
                ).ToList(),
            _context.Moods
                .Where(m => t.Moods.Contains(m.Id))
                .Select(m => new MoodSummary(
                    m.Id.Value,
                    m.Name)
                ).ToList()))
        .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<TrackSummary>> GetAllByGenreIdAsync(
        GenreId genreId,
        CancellationToken cancellationToken = default)
        => await _context.Tracks
        .Where(t => t.Genres.Any(g => g.Value == genreId.Value))
        .Select(t => new TrackSummary(
            t.Id.Value,
            t.Title,
            t.Duration,
            t.ReleaseDate,
            t.ContainsExplicitContent,
            t.Status.Value,
            t.AudioFileId == null ? null : t.AudioFileId.Value,
            t.AlbumId == null ? null : t.AlbumId.Value))
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TrackSummary>> GetAllByMoodIdAsync(
        MoodId moodId,
        CancellationToken cancellationToken = default)
        => await _context.Tracks
        .Where(t => t.Moods.Any(m => m.Value == moodId.Value))
        .Select(t => new TrackSummary(
            t.Id.Value,
            t.Title,
            t.Duration,
            t.ReleaseDate,
            t.ContainsExplicitContent,
            t.Status.Value,
            t.AudioFileId == null ? null : t.AudioFileId.Value,
            t.AlbumId == null ? null : t.AlbumId.Value))
        .ToListAsync(cancellationToken);
}
