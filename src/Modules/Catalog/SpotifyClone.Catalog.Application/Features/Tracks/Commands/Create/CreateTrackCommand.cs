using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.Create;

public sealed record CreateTrackCommand(
    string Title,
    bool ContainsExplicitContent,
    int TrackNumber,
    Guid AlbumId,
    IEnumerable<Guid> MainArtists,
    IEnumerable<Guid> FeaturedArtists,
    IEnumerable<Guid> Genres,
    Guid AudioFileId)
    : IPersistentCommand<CreateTrackCommandResult>;
