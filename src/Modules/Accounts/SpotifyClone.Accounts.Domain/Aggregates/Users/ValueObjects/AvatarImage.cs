using SpotifyClone.Accounts.Domain.Aggregates.Users.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.IDs;
using SpotifyClone.Shared.Kernel.ValueObjects;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users.ValueObjects;

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
            throw new InvalidAvatarImageDomainException("Avatar image must be square.");
        }

        if (!fileType.SupportsTransparency)
        {
            throw new InvalidAvatarImageDomainException("Avatar image must support transparency.");
        }

        ImageId = imageId;
        Metadata = new ImageMetadata(width, height, MaxWidth, MaxHeight, fileType);
    }
}
