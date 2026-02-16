using SpotifyClone.Catalog.Domain.Aggregates.Albums.Enums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Events;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Exceptions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Albums;

public sealed class Album : AggregateRoot<AlbumId, Guid>
{
    private readonly HashSet<ArtistId> _mainArtists = [];
    private readonly HashSet<TrackId> _tracks = [];

    public string Title { get; private set; } = null!;
    public DateTimeOffset? ReleaseDate { get; private set; }
    public AlbumStatus Status { get; private set; } = null!;
    public AlbumType? Type { get; private set; }
    public AlbumCoverImage? Cover { get; private set; }
    public IReadOnlySet<ArtistId> MainArtists => _mainArtists.AsReadOnly();
    public IReadOnlySet<TrackId> Tracks => _tracks.AsReadOnly();

    public static Album Create(AlbumId id, string title, IEnumerable<ArtistId> mainArtists)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(mainArtists);

        AlbumTitleRules.Validate(title);

        if (!mainArtists.Any())
        {
            throw new InvalidAlbumMainArtistsDomainException(
                "An album must have at least one main artist.");
        }

        return new Album(id, title, null, AlbumStatus.Draft, null, null, mainArtists);
    }

    public bool TryMarkAsReadyToPublish()
    {
        if (Cover is null ||
            _tracks.Count <= 0)
        {
            return false;
        }

        Status = AlbumStatus.ReadyToPublish;
        return true;
    }

    public void AttachCover(AlbumCoverImage cover)
    {
        ArgumentNullException.ThrowIfNull(cover);

        if (Cover is not null)
        {
            throw new AlbumAlreadyHaveACoverDomainException(
                "Album is already have a cover. " +
                "Consider to unattach the attached cover first.");
        }

        Cover = cover;
        TryMarkAsReadyToPublish();
    }

    public void UnattachCover()
    {
        if (Cover is null)
        {
            return;
        }

        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException("Cannot unattach published album from it's cover.");
        }

        Cover = null;
        Status = AlbumStatus.Draft;
    }

    public void Publish(DateTimeOffset releaseDate)
    {
        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException("This album has been already published.");
        }

        if (!Status.IsReadyToPublish)
        {
            throw new CannotPublishAlbumDomainException("Album is not ready to publish.");
        }

        ReleaseDate = releaseDate.ToUniversalTime();
        Status = AlbumStatus.Published;

        RaiseDomainEvent(new AlbumPublishedDomainEvent(Id, releaseDate));
    }

    public void AddMainArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException("Cannot add main artist to a published album.");
        }

        _mainArtists.Add(artistId);
    }

    public void RemoveMainArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException(
                "Cannot remove main artist from a published album.");
        }

        if (_mainArtists.Count <= 1)
        {
            throw new InvalidAlbumMainArtistsDomainException(
                "An album must have at least one main artist.");
        }

        _mainArtists.Remove(artistId);
    }

    public bool MainArtistExists(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);
        return _mainArtists.Contains(artistId);
    }

    public void AddTrack(TrackId trackId)
    {
        ArgumentNullException.ThrowIfNull(trackId);

        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException("Cannot add track to a published album.");
        }

        _tracks.Add(trackId);
        Type = AlbumType.From(_tracks.Count);
        TryMarkAsReadyToPublish();
    }

    public void RemoveTrack(TrackId trackId)
    {
        ArgumentNullException.ThrowIfNull(trackId);

        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException("Cannot remove track from a published album.");
        }

        _tracks.Remove(trackId);
        Type = AlbumType.From(_tracks.Count);
    }

    public void ChangeTitle(string newTitle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newTitle);
        AlbumTitleRules.Validate(newTitle);
        Title = newTitle;
    }

    public void ChangeReleaseDate(DateTimeOffset newReleaseDate)
        => ReleaseDate = newReleaseDate;

    public void ChangeCover(AlbumCoverImage newCover)
    {
        ArgumentNullException.ThrowIfNull(newCover);
        Cover = newCover;
    }

    public void PrepareForDeletion()
    {
        if (Status.IsPublished)
        {
            throw new AlbumAlreadyPublishedDomainException("Cannot delete a published album.");
        }
    }

    private Album(
        AlbumId id, string title, DateTimeOffset? releaseDate, AlbumStatus status, AlbumType? type,
        AlbumCoverImage? cover, IEnumerable<ArtistId> mainArtists)
        : base(id)
    {
        Title = title;
        ReleaseDate = releaseDate;
        Status = status;
        Type = type;
        Cover = cover;
        _mainArtists = mainArtists.ToHashSet();
    }

    private Album()
    {
    }
}
