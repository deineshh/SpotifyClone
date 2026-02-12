using SpotifyClone.Catalog.Application.Abstractions;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UpdateInfo;

public sealed record UpdateTrackInfoCommand(
    Guid TrackId,
    string Title,
    DateTimeOffset ReleaseDate,
    bool ContainsExplicitContent,
    int TrackNumber)
    : ICatalogPersistentCommand<UpdateTrackInfoCommandResult>;
