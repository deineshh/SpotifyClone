using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Shared.Kernel.ValueObjects;

namespace SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

public sealed class ImageAsset : AggregateRoot<ImageId, Guid>
{
    public ImageMetadata Metadata { get; private set; }
    public bool IsReady { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ImageAsset()
        => Metadata = null!;

    private ImageAsset(ImageMetadata metadata, bool isReady, DateTimeOffset createdAt)
    {
        Metadata = metadata;
        IsReady = isReady;
        CreatedAt = createdAt;
    }

    public static ImageAsset Create(ImageMetadata metadata, bool isReady)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        return new ImageAsset(metadata, isReady, DateTimeOffset.UtcNow);
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
