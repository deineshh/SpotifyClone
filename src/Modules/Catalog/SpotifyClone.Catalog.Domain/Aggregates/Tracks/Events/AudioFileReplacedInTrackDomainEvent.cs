using SpotifyClone.Catalog.Domain.Aggregates.Tracks.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;

public sealed record AudioFileReplacedInTrackDomainEvent(
    TrackId TrackId,
    AudioFileId AudioFileId,
    TimeSpan Duration)
    : DomainEvent;
