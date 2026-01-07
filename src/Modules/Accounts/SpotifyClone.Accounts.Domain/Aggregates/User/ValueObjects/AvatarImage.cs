using SpotifyClone.Accounts.Domain.Aggregates.User.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.ValueObjects;

namespace SpotifyClone.Accounts.Domain.Aggregates.User.ValueObjects;

public sealed record AvatarImage : ValueObject
{
    private const int MaxWidth = 1024;
    private const int MaxHeight = 1024;

    public ImageMetadata Metadata { get; init; }
    public ImageId ImageId { get; init; }

    public AvatarImage(ImageId imageId, int width, int height, ImageFileType fileType)
    {
        ArgumentNullException.ThrowIfNull(imageId);
        ArgumentNullException.ThrowIfNull(fileType);

        if (width != height)
        {
            throw new AvatarImageInvalidShapeDomainException();
        }

        if (!fileType.SupportsTransparency)
        {
            throw new AvatarImageMustSupportTransparencyDomainException();
        }

        ImageId = imageId;
        Metadata = new ImageMetadata(width, height, MaxWidth, MaxHeight, fileType);
    }
}
