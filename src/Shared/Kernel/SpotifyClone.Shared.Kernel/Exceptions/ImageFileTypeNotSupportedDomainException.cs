using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.Kernel.Exceptions;

public sealed class ImageFileTypeNotSupportedDomainException : DomainExceptionBase
{
    public ImageFileTypeNotSupportedDomainException(string fileType)
        : base($"Image file type '{fileType}' is not supported.")
    {
    }
}
