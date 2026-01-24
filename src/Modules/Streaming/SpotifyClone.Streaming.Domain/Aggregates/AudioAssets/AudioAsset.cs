using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Enums;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.Exceptions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets.ValueObjects;
using SpotifyClone.Streaming.Domain.Exceptions;

namespace SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;

public sealed class AudioAsset : AggregateRoot<AudioAssetId, Guid>
{
    public TimeSpan Duration { get; private set; }
    public AudioFormat Format { get; private set; }
    public long FileSizeInBytes { get; private set; }
    public bool IsReady { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private AudioAsset()
        => Format = null!;

    private AudioAsset(
        AudioAssetId id,
        TimeSpan duration,
        AudioFormat format,
        long fileSizeInBytes,
        bool isReady,
        DateTimeOffset createdAt)
        : base(id)
    {
        Duration = duration;
        Format = format;
        FileSizeInBytes = fileSizeInBytes;
        IsReady = isReady;
        CreatedAt = createdAt;
    }

    public static AudioAsset Create(
        AudioAssetId id,
        TimeSpan duration,
        AudioFormat format,
        long fileSizeInBytes,
        bool isReady)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(format);

        if (duration <= TimeSpan.Zero)
        {
            throw new InvalidDurationDomainException("Duration must be greater than zero.");
        }

        if (fileSizeInBytes <= 0)
        {
            throw new InvalidFileSizeDomainException("File size in bytes must be greater than zero.");
        }

        return new AudioAsset(id, duration, format, fileSizeInBytes, isReady, DateTimeOffset.UtcNow);
    }

    public void MarkAsReady()
    {
        if (IsReady)
        {
            return;
        }

        IsReady = true;

        // Raise domain events if needed
    }
}
