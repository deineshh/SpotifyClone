using Microsoft.EntityFrameworkCore;
using SpotifyClone.Playlists.Application.Abstractions.Data;
using SpotifyClone.Playlists.Application.Features.Playlists.Queries;
using SpotifyClone.Playlists.Application.Models;
using SpotifyClone.Playlists.Domain.Aggregates.Playlists.ValueObjects;
using SpotifyClone.Playlists.Infrastructure.Persistence.Database;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Playlists.Infrastructure.Persistence.Queries;

internal sealed class PlaylistEfCoreReadService(
    PlaylistsAppDbContext context)
    : IPlaylistReadService
{
    private readonly PlaylistsAppDbContext _context = context;

    public async Task<PlaylistDetails?> GetDetailsAsync(
        PlaylistId id,
        CancellationToken cancellationToken = default)
        => await _context.Playlists
        .Where(p => p.Id == id)
        .Select(p => new PlaylistDetails(
            p.Id.Value,
            p.Name,
            p.Description,
            p.OwnerId.Value,
            p.IsPublic,
            p.Cover == null ? null : new ImageMetadataDetails(
                p.Cover.ImageId.Value,
                p.Cover.Metadata.Width,
                p.Cover.Metadata.Height,
                p.Cover.Metadata.FileType.Value,
                p.Cover.Metadata.SizeInBytes),
            p.Collaborators.Select(id => id.Value),
            p.Tracks.Select(t => new PlaylistTrackSummary(t.Id.Value, t.Position))))
        .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<PlaylistSummary>> GetAllByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
        => await _context.Playlists
        .Where(p => p.OwnerId == ownerId)
        .Select(p => new PlaylistSummary(
            p.Id.Value,
            p.Name,
            p.Description,
            p.IsPublic,
            p.Cover == null ? null : new ImageMetadataDetails(
                p.Cover.ImageId.Value,
                p.Cover.Metadata.Width,
                p.Cover.Metadata.Height,
                p.Cover.Metadata.FileType.Value,
                p.Cover.Metadata.SizeInBytes)))
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlaylistSummary>> GetAllPublicByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
        => await _context.Playlists
        .Where(p => p.OwnerId == ownerId && p.IsPublic)
        .Select(p => new PlaylistSummary(
            p.Id.Value,
            p.Name,
            p.Description,
            p.IsPublic,
            p.Cover == null ? null : new ImageMetadataDetails(
                p.Cover.ImageId.Value,
                p.Cover.Metadata.Width,
                p.Cover.Metadata.Height,
                p.Cover.Metadata.FileType.Value,
                p.Cover.Metadata.SizeInBytes)))
        .ToListAsync(cancellationToken);
}
