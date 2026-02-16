using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Enums;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Events;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Exceptions;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.Rules;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks;

public sealed class Track : AggregateRoot<TrackId, Guid>
{
    private readonly HashSet<ArtistId> _mainArtists = [];
    private readonly HashSet<ArtistId> _featuredArtists = [];
    private readonly HashSet<GenreId> _genres = [];
    private readonly HashSet<MoodId> _moods = [];

    public string Title { get; private set; } = null!;
    public TimeSpan? Duration { get; private set; }
    public DateTimeOffset? ReleaseDate { get; private set; }
    public bool ContainsExplicitContent { get; private set; }
    public int TrackNumber { get; private set; }
    public TrackStatus Status { get; private set; } = null!;
    public AudioFileId? AudioFileId { get; private set; }
    public AlbumId AlbumId { get; private set; } = null!;
    public IReadOnlySet<ArtistId> MainArtists => _mainArtists.AsReadOnly();
    public IReadOnlySet<ArtistId> FeaturedArtists => _featuredArtists.AsReadOnly();
    public IReadOnlySet<GenreId> Genres => _genres.AsReadOnly();
    public IReadOnlySet<MoodId> Moods => _moods.AsReadOnly();

    public static Track Create(
        TrackId id,
        string title,
        bool containsExplicitContent,
        int trackNumber,
        AlbumId albumId,
        IEnumerable<ArtistId> mainArtists,
        IEnumerable<ArtistId> featuredArtists,
        IEnumerable<GenreId> genres,
        IEnumerable<MoodId> moods)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(trackNumber);
        ArgumentNullException.ThrowIfNull(albumId);
        ArgumentNullException.ThrowIfNull(mainArtists);
        ArgumentNullException.ThrowIfNull(featuredArtists);
        ArgumentNullException.ThrowIfNull(genres);
        ArgumentNullException.ThrowIfNull(moods);

        TrackTitleRules.Validate(title);

        if (!mainArtists.Any())
        {
            throw new InvalidTrackMainArtistsDomainException(
                "A track must have at least one main artist.");
        }

        if (!genres.Any())
        {
            throw new InvalidTrackGenresDomainException(
                "A track must have at least one genre.");
        }

        if (!moods.Any())
        {
            throw new InvalidTrackMoodsDomainException(
                "A track must have at least one mood.");
        }

        var track = new Track(
            id, title, null, null, containsExplicitContent, trackNumber, TrackStatus.Draft, null, albumId,
            mainArtists, featuredArtists, genres, moods);

        return track;
    }

    public void MarkAsReadyToPublish()
        => Status = TrackStatus.ReadyToPublish;

    public void LinkAudioFile(AudioFileId audioFileId, TimeSpan duration)
    {
        ArgumentNullException.ThrowIfNull(audioFileId);

        if (AudioFileId is not null)
        {
            throw new TrackAlreadyLinkedToAudioFileDomainException(
                "Track is already linked to an audio file. " +
                "Consider to unlink the linked audio file first.");
        }

        TrackDurationRules.Validate(duration);

        Duration = duration;
        AudioFileId = audioFileId;
    }

    public void UnlinkAudioFile()
    {
        if (AudioFileId is null)
        {
            return;
        }

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException("Cannot unlink published track from it's audio file.");
        }

        RaiseDomainEvent(new TrackUnlinkedFromAudioFileDomainEvent(AudioFileId));

        AudioFileId = null;
        Status = TrackStatus.Draft;
    }

    public void Publish(DateTimeOffset releaseDate)
    {
        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException("This track has been already published.");
        }

        if (!Status.IsReadyToPublish)
        {
            throw new CannotPublishTrackDomainException("This track is not ready to publish.");
        }

        if (AudioFileId is null)
        {
            throw new CannotPublishTrackDomainException("Cannot publish track while audio file is not linked.");
        }

        if (releaseDate < DateTimeOffset.UtcNow.AddMinutes(-1))
        {
            throw new InvalidTrackReleaseDateDomainException("Track release date cannot be in the past.");
        }

        ReleaseDate = releaseDate.ToUniversalTime();
        Status = TrackStatus.Published;
    }

    public void Unpublish()
    {
        if (!Status.IsPublished)
        {
            throw new TrackNotPublishedDomainException("Cannot unpublish track which is not published.");
        }

        ReleaseDate = null;
        Status = TrackStatus.ReadyToPublish;
    }

    public void PrepareForDeletion()
    {
        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException("Cannot delete a published track.");
        }

        if (AudioFileId is not null)
        {
            RaiseDomainEvent(new TrackDeletedDomainEvent(AudioFileId));
        }
    }

    public void CorrectTitle(string title)
    {
        if (title == Title)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        TrackTitleRules.Validate(title);
        Title = title;
    }

    public void RescheduleRelease(DateTimeOffset releaseDate)
    {
        if (releaseDate == ReleaseDate)
        {
            return;
        }

        if (!Status.IsPublished)
        {
            throw new TrackNotPublishedDomainException("Release date cannot be set before publishing the track.");
        }

        if (DateTimeOffset.UtcNow >= ReleaseDate)
        {
            throw new TrackAlreadyReleasedDomainException("Cannot reschedule a release of a released track.");
        }

        ReleaseDate = releaseDate.ToUniversalTime();
    }

    public void MoveToPosition(int trackNumber)
    {
        if (trackNumber == TrackNumber)
        {
            return;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(trackNumber);
        TrackNumber = trackNumber;
    }
    public void MarkAsExplicit()
        => ContainsExplicitContent = true;

    public void MarkAsNotExplicit()
        => ContainsExplicitContent = false;

    public void AddMainArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException("Cannot add main artist to a published track.");
        }

        _mainArtists.Add(artistId);
    }

    public void RemoveMainArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove main artist from a published track.");
        }

        _mainArtists.Remove(artistId);
    }

    public void AddFeaturedArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot add featured artist to a published track.");
        }

        _featuredArtists.Add(artistId);
    }

    public void RemoveFeaturedArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove featured artist from a published track.");
        }

        _featuredArtists.Remove(artistId);
    }

    public void AddGenre(GenreId genreId)
    {
        ArgumentNullException.ThrowIfNull(genreId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot add genre to a published track.");
        }

        _genres.Add(genreId);
    }

    public void RemoveGenre(GenreId genreId)
    {
        ArgumentNullException.ThrowIfNull(genreId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove genre from a published track.");
        }

        _genres.Remove(genreId);
    }

    public void AddMood(MoodId moodId)
    {
        ArgumentNullException.ThrowIfNull(moodId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot add mood to a published track.");
        }

        _moods.Add(moodId);
    }

    public void RemoveMood(MoodId moodId)
    {
        ArgumentNullException.ThrowIfNull(moodId);

        if (Status.IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove mood from a published track.");
        }

        _moods.Remove(moodId);
    }

    public bool MainArtistExists(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);
        return _mainArtists.Contains(artistId);
    }

    public bool FeaturedArtistExists(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);
        return _featuredArtists.Contains(artistId);
    }

    public bool GenreExists(GenreId genreId)
    {
        ArgumentNullException.ThrowIfNull(genreId);
        return _genres.Contains(genreId);
    }

    public bool MoodExists(MoodId moodId)
    {
        ArgumentNullException.ThrowIfNull(moodId);
        return _moods.Contains(moodId);
    }

    private Track(
        TrackId id,
        string title,
        TimeSpan? duration,
        DateTimeOffset? releaseDate,
        bool containsExplicitContent,
        int trackNumber,
        TrackStatus status,
        AudioFileId? audioFileId,
        AlbumId albumId,
        IEnumerable<ArtistId> mainArtists,
        IEnumerable<ArtistId> featuredArtists,
        IEnumerable<GenreId> genres,
        IEnumerable<MoodId> moods)
        : base(id)
    {
        Title = title;
        Duration = duration;
        ReleaseDate = releaseDate;
        ContainsExplicitContent = containsExplicitContent;
        TrackNumber = trackNumber;
        Status = status;
        AudioFileId = audioFileId;
        AlbumId = albumId;
        _mainArtists = mainArtists.ToHashSet();
        _featuredArtists = featuredArtists.ToHashSet();
        _genres = genres.ToHashSet();
        _moods = moods.ToHashSet();
    }

    private Track()
    {
    }
}
