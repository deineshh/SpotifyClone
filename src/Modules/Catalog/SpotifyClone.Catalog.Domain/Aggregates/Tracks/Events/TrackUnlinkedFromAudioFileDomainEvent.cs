using SpotifyClone.Catalog.Domain.Aggregates.Tracks.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;

public sealed record TrackUnlinkedFromAudioFileDomainEvent(
    AudioFileId AudioFileId)
    : DomainEvent;
