namespace SpotifyClone.Catalog.Application.Models;

public sealed record ImageMetadataDetailsResult(
    Guid ImageId,
    int Width,
    int Height,
    string FileType,
    long SizeInBytes);
