using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.Kernel.ValueObjects;

public sealed record ImageMetadata : ValueObject
{
    public int Width { get; }
    public int Height { get; }
    public string FileType { get; }

    public ImageMetadata(int width, int height, string fileType)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileType);

        Width = width;
        Height = height;
        FileType = NormalizeFileType(fileType);
    }

    private static string NormalizeFileType(string fileType)
        => fileType.Trim().ToLowerInvariant();
}
