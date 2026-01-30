using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Enums;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;

namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;

public sealed class AudioAsset : AggregateRoot<AudioAssetId, Guid>
{
    public TimeSpan? Duration { get; private set; }
    public AudioFormat? Format { get; private set; }
    public long? SizeInBytes { get; private set; }
    public bool IsReady { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static AudioAsset Create(
        AudioAssetId id,
        bool isReady,
        TimeSpan? duration,
        AudioFormat? format,
        long? sizeInBytes)
    {
        ArgumentNullException.ThrowIfNull(id);

        return new AudioAsset(id, duration, format, sizeInBytes, isReady, DateTimeOffset.UtcNow);
    }

    public void MarkAsReady(TimeSpan duration, AudioFormat format, long sizeInBytes)
    {
        if (IsReady)
        {
            return;
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

        // Raise domain events if needed
    }

    private AudioAsset(
        AudioAssetId id,
        TimeSpan? duration,
        AudioFormat? format,
        long? sizeInBytes,
        bool isReady,
        DateTimeOffset createdAt)
        : base(id)
    {
        Duration = duration;
        Format = format;
        SizeInBytes = sizeInBytes;
        IsReady = isReady;
        CreatedAt = createdAt;
    }

    private AudioAsset()
    {
    }
}
