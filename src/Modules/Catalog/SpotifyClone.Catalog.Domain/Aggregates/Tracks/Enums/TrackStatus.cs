using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks.Enums;

public sealed record TrackStatus : ValueObject
{
    public static readonly TrackStatus Draft = new("draft");
    public static readonly TrackStatus ReadyToPublish = new("ready_to_publish");
    public static readonly TrackStatus Published = new("published");
    public static readonly TrackStatus Archived = new("archived");

    public bool IsDraft => this == Draft;
    public bool IsReadyToPublish => this == ReadyToPublish;
    public bool IsPublished => this == Published;

    public string Value { get; }

    private TrackStatus(string value)
        => Value = value;

    public static TrackStatus From(string value)
        => value.ToLowerInvariant() switch
        {
            "draft" => Draft,
            "ready_to_publish" => ReadyToPublish,
            "published" => Published,
            "archived" => Archived,
            _ => throw new InvalidTrackStatusDomainException($"Invalid track status value: {value}")
        };
}
