using SpotifyClone.Catalog.Application.Abstractions;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.PublishTrack;

public sealed record PublishTrackCommand(
    Guid TrackId,
    DateTimeOffset ReleaseDate)
    : ICatalogPersistentCommand<PublishTrackCommandResult>;
