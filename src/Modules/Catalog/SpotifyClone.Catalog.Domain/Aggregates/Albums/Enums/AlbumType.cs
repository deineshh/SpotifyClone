using SpotifyClone.Catalog.Domain.Aggregates.Albums.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Albums.Enums;

public sealed record AlbumType : ValueObject
{
    private const ushort MaxTracksForSingle = 3;
    private const ushort MaxTracksForExtendedPlay = 6;
    private const ushort MaxTracksForLongPlay = 50;

    public static readonly AlbumType Single = new("Single");
    public static readonly AlbumType ExtendedPlay = new("ExtendedPlay");
    public static readonly AlbumType LongPlay = new("LongPlay");

    public string Value { get; }

    private AlbumType(string value)
        => Value = value;

    public static AlbumType? From(int trackCount)
    {
        if (trackCount <= MaxTracksForSingle)
        {
            return Single;
        }
        else if (trackCount <= MaxTracksForExtendedPlay)
        {
            return ExtendedPlay;
        }
        else if (trackCount <= MaxTracksForLongPlay)
        {
            return LongPlay;
        }
        else
        {
            throw new InvalidAlbumTypeDomainException(
                $"Track count must be between 1 and {MaxTracksForLongPlay}.");
        }
    }
}
