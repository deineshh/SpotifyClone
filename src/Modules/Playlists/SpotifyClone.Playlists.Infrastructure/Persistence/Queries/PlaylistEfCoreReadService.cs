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
    {
        var header = await _context.Playlists
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.OwnerId,
                p.IsPublic,
                p.Cover,
                TrackInfos = p.Tracks.OrderBy(t => t.Position)
                    .Select(t => new { t.Id, t.Position }).ToList(),
                CollaboratorIds = p.Collaborators.Select(c => c.Value).ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (header == null)
        {
            return null;
        }

        var top4Ids = header.TrackInfos.Take(4).Select(x => x.Id.Value).ToList();

        List<CollaboratorSummary> collaboratorsTask = await _context.UserReferences
            .AsNoTracking()
            .Where(u => header.CollaboratorIds.Contains(u.Id))
            .Select(u => new CollaboratorSummary(u.Id, u.Name, u.AvatarImageId))
            .ToListAsync(cancellationToken);

        List<Guid> validCoverIdsTask = await _context.TrackReferences
            .AsNoTracking()
            .Where(t => top4Ids.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        return new PlaylistDetails(
            header.Id.Value,
            header.Name,
            header.Description,
            header.OwnerId.Value,
            header.IsPublic,
            header.Cover == null ? null : new ImageMetadataDetails(
                header.Cover.ImageId.Value,
                header.Cover.Metadata.Width,
                header.Cover.Metadata.Height,
                header.Cover.Metadata.FileType.Value,
                header.Cover.Metadata.SizeInBytes),
            validCoverIdsTask
                .OrderBy(id => header.TrackInfos.FindIndex(x => x.Id.Value == id))
                .ToList(),
            collaboratorsTask,
            header.TrackInfos.Select(t => new PlaylistTrackSummary(t.Id.Value, t.Position)).ToList()
        );
    }

    public async Task<IEnumerable<PlaylistSummary>> GetAllByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
        => await _context.Playlists
        .AsNoTracking()
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
                p.Cover.Metadata.SizeInBytes),
            _context.TrackReferences
                .Where(t => p.Tracks.Any(pt => pt.Id.Value == t.Id))
                .OrderBy(t => p.Tracks.First(pt => pt.Id.Value == t.Id).Position)
                .Select(t => t.Id)
                .Take(4)))
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
                p.Cover.Metadata.SizeInBytes),
            _context.TrackReferences
                .Where(t => p.Tracks.Any(pt => pt.Id.Value == t.Id))
                .OrderBy(t => p.Tracks.First(pt => pt.Id.Value == t.Id).Position)
                .Select(t => t.Id)
                .Take(4)))
        .ToListAsync(cancellationToken);
}
