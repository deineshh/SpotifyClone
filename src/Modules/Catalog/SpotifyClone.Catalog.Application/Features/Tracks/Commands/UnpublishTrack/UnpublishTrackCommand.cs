using SpotifyClone.Catalog.Application.Abstractions;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnpublishTrack;

public sealed record UnpublishTrackCommand(
    Guid TrackId)
    : ICatalogPersistentCommand<UnpublishTrackCommandResult>;
