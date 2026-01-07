using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.Kernel.Exceptions;

public sealed class ImageTooLargeDomainException : DomainExceptionBase
{
    public ImageTooLargeDomainException(int maxWidth, int maxHeight)
        : base($"Image must not exceed {maxWidth}x{maxHeight}.")
    {
    }
}
