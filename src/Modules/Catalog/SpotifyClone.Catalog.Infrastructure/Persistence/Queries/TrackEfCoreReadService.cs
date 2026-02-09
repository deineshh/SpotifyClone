using Microsoft.EntityFrameworkCore;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Genres.Queries;
using SpotifyClone.Catalog.Application.Features.Moods.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Models;
using SpotifyClone.Catalog.Infrastructure.Persistence.Database;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Queries;

internal sealed class TrackEfCoreReadService(
    CatalogAppDbContext context) : ITrackReadService
{
    private readonly CatalogAppDbContext _context = context;

    public async Task<TrackDetailsResponse?> GetDetailsAsync(
        TrackId id,
        CancellationToken cancellationToken = default)
        => await _context.Tracks
        .Where(t => t.Id == id)
        .Select(t => new TrackDetailsResponse(
            t.Title,
            t.Duration,
            t.ReleaseDate,
            t.ContainsExplicitContent,
            t.TrackNumber,
            t.Status.Value,
            t.AudioFileId == null ? null : t.AudioFileId.Value,
            t.AlbumId.Value,
            _context.Artists
                .Where(a => t.MainArtists.Contains(a.Id))
                .Select(a => new ArtistSummaryResult(
                    a.Name,
                    a.IsVerified,
                    a.Avatar == null ? null : new ImageMetadataDetailsResult(
                        a.Avatar.ImageId.Value,
                        a.Avatar.Metadata.Width!.Value,
                        a.Avatar.Metadata.Height!.Value,
                        a.Avatar.Metadata.FileType!.Value,
                        a.Avatar.Metadata.SizeInBytes!.Value))
                ).ToList(),
            _context.Artists
                .Where(a => t.FeaturedArtists.Contains(a.Id))
                .Select(a => new ArtistSummaryResult(
                    a.Name,
                    a.IsVerified,
                    a.Avatar == null ? null : new ImageMetadataDetailsResult(
                        a.Avatar.ImageId.Value,
                        a.Avatar.Metadata.Width!.Value,
                        a.Avatar.Metadata.Height!.Value,
                        a.Avatar.Metadata.FileType!.Value,
                        a.Avatar.Metadata.SizeInBytes!.Value))
                ).ToList(),
            _context.Genres
                .Where(g => t.Genres.Contains(g.Id))
                .Select(g => new GenreSummaryResult(g.Name)).ToList(),
            _context.Moods
                .Where(m => t.Moods.Contains(m.Id))
                .Select(m => new MoodSummaryResult(m.Name)).ToList()))
        .SingleOrDefaultAsync(cancellationToken);
}
