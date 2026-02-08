using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;

namespace SpotifyClone.Shared.IntegrationEvents.Streaming.AudioAssets;

public sealed record AudioReadyIntegrationEvent(
    Guid TrackId)
    : IntegrationEvent;
