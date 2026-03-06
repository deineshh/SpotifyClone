using SpotifyClone.Playlists.Application.Models;

namespace SpotifyClone.Playlists.Application.Features.Playlists.Queries;

public sealed record PlaylistDetails(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    bool IsPublic,
    ImageMetadataDetails? Cover,
    IEnumerable<Guid> Contributors,
    IEnumerable<PlaylistTrackSummary> Tracks);
