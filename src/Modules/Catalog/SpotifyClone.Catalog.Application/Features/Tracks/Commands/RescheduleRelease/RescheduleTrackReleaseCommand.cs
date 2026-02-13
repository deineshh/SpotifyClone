using SpotifyClone.Catalog.Application.Abstractions;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.RescheduleRelease;

public sealed record RescheduleTrackReleaseCommand(
    Guid TrackId,
    DateTimeOffset ReleaseDate)
    : ICatalogPersistentCommand<RescheduleTrackReleaseCommandResult>;
