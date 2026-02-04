using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;

namespace SpotifyClone.Shared.IntegrationEvents.Catalog.Tracks;

public sealed record TrackCreatedIntegrationEvent(
    Guid TrackId,
    Guid AudioFileId)
    : IntegrationEvent;
