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
        .Where(a => a.Id == id)
        .Select(a => new PlaylistDetails(
            a.Id.Value,
            a.Name,
            a.Description,
            a.OwnerId.Value,
            a.IsPublic,
            a.Cover == null ? null : new ImageMetadataDetails(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes),
            a.Collaborators.Select(id => id.Value),
            a.Tracks.Select(t => new PlaylistTrackSummary(t.Id.Value, t.Position))))
        .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<PlaylistSummary>> GetAllByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
        => await _context.Playlists
        .Where(a => a.OwnerId.Value == ownerId.Value)
        .Select(a => new PlaylistSummary(
            a.Id.Value,
            a.Name,
            a.Description,
            a.IsPublic,
            a.Cover == null ? null : new ImageMetadataDetails(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes)))
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlaylistSummary>> GetAllPublicByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
        => await _context.Playlists
        .Where(a => a.OwnerId.Value == ownerId.Value && a.IsPublic)
        .Select(a => new PlaylistSummary(
            a.Id.Value,
            a.Name,
            a.Description,
            a.IsPublic,
            a.Cover == null ? null : new ImageMetadataDetails(
                a.Cover.ImageId.Value,
                a.Cover.Metadata.Width,
                a.Cover.Metadata.Height,
                a.Cover.Metadata.FileType.Value,
                a.Cover.Metadata.SizeInBytes)))
        .ToListAsync(cancellationToken);
}
