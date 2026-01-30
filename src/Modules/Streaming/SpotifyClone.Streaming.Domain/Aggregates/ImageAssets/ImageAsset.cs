using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Shared.Kernel.ValueObjects;

namespace SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

public sealed class ImageAsset : AggregateRoot<ImageId, Guid>
{
    public ImageMetadata? Metadata { get; private set; }
    public bool IsReady { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static ImageAsset Create(ImageId id, bool isReady, ImageMetadata? metadata)
    {
        ArgumentNullException.ThrowIfNull(id);

        return new ImageAsset(id, metadata, isReady, DateTimeOffset.UtcNow);
    }

    public void MarkAsReady(ImageMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        if (IsReady)
        {
            return;
        }

        Metadata = metadata;
        IsReady = true;

        // Raise domain events if needed
    }

    private ImageAsset(ImageId id, ImageMetadata? metadata, bool isReady, DateTimeOffset createdAt)
        : base(id)
    {
        Metadata = metadata;
        IsReady = isReady;
        CreatedAt = createdAt;
    }

    private ImageAsset()
    {
    }
}
