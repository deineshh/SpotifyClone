using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Enums;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Events;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;

public sealed class AudioAsset : AggregateRoot<AudioAssetId, Guid>
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(24);

    public TimeSpan Duration { get; private set; }
    public AudioFormat? Format { get; private set; }
    public long? SizeInBytes { get; private set; }
    public bool IsReady { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public TrackId? TrackId { get; private set; }

    public static AudioAsset Create(
        AudioAssetId id,
        bool isReady,
        TimeSpan duration,
        AudioFormat? format,
        long? sizeInBytes,
        TrackId trackId)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(trackId);

        if (duration > MaxDuration)
        {
            throw new InvalidDurationDomainException($"Duration cannot exceed {MaxDuration.TotalHours} hours.");
        }

        var audioAsset = new AudioAsset(
            id, duration, format, sizeInBytes, isReady, DateTimeOffset.UtcNow, trackId);

        audioAsset.RaiseDomainEvent(new AudioAssetCreatedDomainEvent(id, trackId, duration));

        return audioAsset;
    }

    public void MarkAsReady(TimeSpan duration, AudioFormat format, long sizeInBytes)
    {
        if (IsReady)
        {
            return;
        }

        if (TrackId is null)
        {
            throw new TrackNotLinkedDomainException(
                "Audio Asset cannot be ready without a linked Track.");
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new InvalidDurationDomainException("Duration must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(format);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);

        Duration = duration;
        Format = format;
        SizeInBytes = sizeInBytes;
        IsReady = true;

        RaiseDomainEvent(new AudioAssetReadyDomainEvent(TrackId));
    }

    public void UnlinkFromTrack()
        => TrackId = null;

    private AudioAsset(
        AudioAssetId id,
        TimeSpan duration,
        AudioFormat? format,
        long? sizeInBytes,
        bool isReady,
        DateTimeOffset createdAt,
        TrackId? trackId)
        : base(id)
    {
        Duration = duration;
        Format = format;
        SizeInBytes = sizeInBytes;
        IsReady = isReady;
        CreatedAt = createdAt;
        TrackId = trackId;
    }

    private AudioAsset()
    {
    }
}
