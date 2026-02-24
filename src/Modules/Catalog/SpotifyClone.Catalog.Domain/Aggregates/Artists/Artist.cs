using SpotifyClone.Catalog.Domain.Aggregates.Artists.Enums;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Events;
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
    public ArtistStatus Status { get; private set; } = null!;
    public ArtistAvatarImage? Avatar { get; private set; }
    public ArtistBannerImage? Banner { get; private set; }
    public IReadOnlySet<ArtistGalleryImage> Gallery => _gallery.AsReadOnly();

    public static Artist Create(ArtistId id, string name)
    {
        ArgumentNullException.ThrowIfNull(id);

        ArtistNameRules.Validate(name);

        return new Artist(id, name, null, ArtistStatus.NotVerified, null, null, []);
    }

    public void LinkNewAvatar(ArtistAvatarImage avatar)
    {
        ArgumentNullException.ThrowIfNull(avatar);

        if (Avatar is not null)
        {
            RaiseDomainEvent(new ArtistUnlinkedFromAvatarImageDomainEvent(Avatar.ImageId));
            Avatar = null;
        }

        Avatar = avatar;
        RaiseDomainEvent(new ArtistLinkedToAvatarImageDomainEvent(Avatar.ImageId));
    }

    public void LinkNewBanner(ArtistBannerImage banner)
    {
        ArgumentNullException.ThrowIfNull(banner);

        if (Banner is not null)
        {
            RaiseDomainEvent(new ArtistUnlinkedFromBannerImageDomainEvent(Banner.ImageId));
            Banner = null;
        }

        Banner = banner;
        RaiseDomainEvent(new ArtistLinkedToBannerImageDomainEvent(Banner.ImageId));
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
        Status = ArtistStatus.Verified;
        Avatar = avatar;
        Banner = banner;
        _gallery = gallery.ToHashSet();
    }

    public void Unverify()
    {
        Status = ArtistStatus.NotVerified;
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
        if (!Status.IsVerified)
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
        if (!Status.IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have an avatar.");
        }

        Avatar = avatar;
    }

    public void ChangeBanner(ArtistBannerImage? banner)
    {
        if (!Status.IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have a banner.");
        }

        Banner = banner;
    }

    public void AddGalleryImage(ArtistGalleryImage image)
    {
        if (!Status.IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have gallery images.");
        }

        _gallery.Add(image);
    }

    public void RemoveGalleryImage(ArtistGalleryImage image)
        => _gallery.Remove(image);

    public void Ban()
    {
        if (Status.IsBanned)
        {
            return;
        }

        Status = ArtistStatus.Banned;
        RaiseDomainEvent(new ArtistBannedDomainEvent(Id));
    }

    public void Unban()
    {
        if (!Status.IsBanned)
        {
            return;
        }

        Status = ArtistStatus.NotVerified;
    }

    private Artist(
        ArtistId id,
        string name,
        string? bio,
        ArtistStatus status,
        ArtistAvatarImage? avatar,
        ArtistBannerImage? banner,
        IEnumerable<ArtistGalleryImage> gallery)
        : base(id)
    {
        Name = name;
        Bio = bio;
        Status = status;
        Avatar = avatar;
        Banner = banner;
        _gallery = gallery.ToHashSet();
    }

    private Artist()
    {
    }
}
