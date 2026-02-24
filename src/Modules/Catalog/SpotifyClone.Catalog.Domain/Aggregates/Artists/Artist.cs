using SpotifyClone.Catalog.Domain.Aggregates.Artists.Enums;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Events;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Exceptions;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Catalog.Domain.Aggregates.Artists;

public sealed class Artist : AggregateRoot<ArtistId, Guid>
{
    private readonly HashSet<ArtistGalleryImage> _gallery = [];

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

        ThrowIfBanned("Cannot link a new avatar image to a banned artist.");

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

        ThrowIfBanned("Cannot link a new banner image to a banned artist.");

        if (Banner is not null)
        {
            RaiseDomainEvent(new ArtistUnlinkedFromBannerImageDomainEvent(Banner.ImageId));
            Banner = null;
        }

        Banner = banner;
        RaiseDomainEvent(new ArtistLinkedToBannerImageDomainEvent(Banner.ImageId));
    }

    public void Verify()
    {
        ThrowIfBanned("Cannot verify a banned artist.");

        if (Status.IsVerified)
        {
            return;
        }

        Status = ArtistStatus.Verified;
    }

    public void Unverify()
    {
        ThrowIfBanned("Cannot unverify a banned artist.");

        if (!Status.IsVerified)
        {
            return;
        }

        Status = ArtistStatus.NotVerified;
    }

    public void Rename(string name)
    {
        ThrowIfBanned("Cannot rename a banned artist.");

        ArtistNameRules.Validate(name);
        Name = name;
    }

    public void UpdateBio(string? bio)
    {
        ThrowIfBanned("Cannot update the bio of a banned artist.");

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

    public void AddGalleryImage(ArtistGalleryImage image)
    {
        ThrowIfBanned("Cannot add a gallery image to a banned artist.");

        if (!Status.IsVerified)
        {
            throw new ArtistNotVerifiedDomainException("Only verified artists can have gallery images.");
        }

        _gallery.Add(image);
    }

    public void RemoveGalleryImage(ArtistGalleryImage image)
    {
        ThrowIfBanned("Cannot remove a gallery image from a banned artist.");

        _gallery.Remove(image);
    }

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

    private void ThrowIfBanned(string message)
    {
        if (Status.IsBanned)
        {
            throw new ArtistBannedDomainException(message);
        }
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
