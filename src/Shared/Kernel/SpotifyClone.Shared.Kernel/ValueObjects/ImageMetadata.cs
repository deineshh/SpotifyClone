using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.Enums;

namespace SpotifyClone.Shared.Kernel.ValueObjects;

public sealed record ImageMetadata : ValueObject
{
    public int? Width { get; }
    public int? Height { get; }
    public ImageFileType? FileType { get; }
    public long? SizeInBytes { get; }

    private ImageMetadata()
        => FileType = null!;

    public ImageMetadata(
        int width,
        int height,
        ImageFileType fileType,
        long sizeInBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentNullException.ThrowIfNull(fileType);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);

        Width = width;
        Height = height;
        FileType = fileType;
        SizeInBytes = sizeInBytes;
    }
}
