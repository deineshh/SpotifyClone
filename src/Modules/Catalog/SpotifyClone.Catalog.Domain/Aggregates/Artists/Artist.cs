using SpotifyClone.Catalog.Domain.Aggregates.Artists.Exceptions;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Artists;

public sealed class Artist : AggregateRoot<ArtistId, Guid>
{
    private HashSet<ArtistGalleryImage> _gallery = [];

    public string Name { get; private set; } = null!;
    public string? Bio { get; private set; }
    public bool IsVerified { get; private set; }
    public ArtistAvatarImage? Avatar { get; private set; } = null!;
    public ArtistBannerImage? Banner { get; private set; } = null!;
    public IReadOnlySet<ArtistGalleryImage> Gallery => _gallery.AsReadOnly();

    public static Artist Create(ArtistId id, string name)
    {
        ArgumentNullException.ThrowIfNull(id);

        ArtistNameRules.Validate(name);

        return new Artist(id, name, null, false, null, null, []);
    }

    public void Verify(
        string? bio, ArtistAvatarImage? avatar, ArtistBannerImage? banner, IEnumerable<ArtistGalleryImage> gallery)
    {
        if (string.IsNullOrEmpty(bio))
        {
            bio = null;
        }
        ArtistBioRules.Validate(bio);

        Bio = bio;
        IsVerified = true;
        Avatar = avatar;
        Banner = banner;
        _gallery = gallery.ToHashSet();
    }

    public void Unverify()
    {
        IsVerified = false;
        Bio = null;
        Avatar = null;
        Banner = null;
        _gallery.Clear();
    }

    public void ChangeName(string name)
    {
        ArtistNameRules.Validate(name);
        Name = name;
    }

    public void ChangeBio(string? bio)
    {
        if (!IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have a bio.");
        }

        if (string.IsNullOrEmpty(bio))
        {
            bio = null;
        }
        ArtistBioRules.Validate(bio);

        Bio = bio;
    }

    public void ChangeAvatar(ArtistAvatarImage? avatar)
    {
        if (!IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have an avatar.");
        }

        Avatar = avatar;
    }

    public void ChangeBanner(ArtistBannerImage? banner)
    {
        if (!IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have a banner.");
        }

        Banner = banner;
    }

    public void AddGalleryImage(ArtistGalleryImage image)
    {
        if (!IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have gallery images.");
        }

        _gallery.Add(image);
    }

    public void RemoveGalleryImage(ArtistGalleryImage image)
        => _gallery.Remove(image);

    private Artist(
        ArtistId id, string name, string? bio, bool isVerified, ArtistAvatarImage? avatar, ArtistBannerImage? banner,
        IEnumerable<ArtistGalleryImage> gallery)
        : base(id)
    {
        Name = name;
        Bio = bio;
        IsVerified = isVerified;
        Avatar = avatar;
        Banner = banner;
        _gallery = gallery.ToHashSet();
    }

    private Artist()
    {
    }
}
