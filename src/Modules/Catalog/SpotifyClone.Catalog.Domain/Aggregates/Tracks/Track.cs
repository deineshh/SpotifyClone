using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
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

    public string Title { get; private set; } = null!;
    public TimeSpan? Duration { get; private set; }
    public DateTimeOffset? ReleaseDate { get; private set; }
    public bool ContainsExplicitContent { get; private set; }
    public int TrackNumber { get; private set; }
    public bool IsPublished { get; private set; }
    public AudioFileId? AudioFileId { get; private set; } = null!;
    public AlbumId AlbumId { get; private set; } = null!;
    public IReadOnlySet<ArtistId> MainArtists => _mainArtists.AsReadOnly();
    public IReadOnlySet<ArtistId> FeaturedArtists => _featuredArtists.AsReadOnly();
    public IReadOnlySet<GenreId> Genres => _genres.AsReadOnly();

    public static Track Create(
        TrackId id,
        string title,
        bool containsExplicitContent,
        int trackNumber,
        AlbumId albumId,
        IEnumerable<ArtistId> mainArtists,
        IEnumerable<ArtistId> featuredArtists,
        IEnumerable<GenreId> genres)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(trackNumber);
        ArgumentNullException.ThrowIfNull(albumId);
        ArgumentNullException.ThrowIfNull(mainArtists);
        ArgumentNullException.ThrowIfNull(featuredArtists);
        ArgumentNullException.ThrowIfNull(genres);

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

        return new(
            id, title, null, null, containsExplicitContent, trackNumber, false, null, albumId,
            mainArtists, featuredArtists, genres);
    }

    public void Publish(AudioFileId audioFileId, TimeSpan duration, DateTimeOffset releaseDate)
    {
        ArgumentNullException.ThrowIfNull(audioFileId);

        if (IsPublished)
        {
            return;
        }

        TrackDurationRules.Validate(duration);

        AudioFileId = audioFileId;
        Duration = duration;
        ReleaseDate = releaseDate;
        IsPublished = true;
    }

    public void AddMainArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException("Cannot add main artist to a published track.");
        }

        _mainArtists.Add(artistId);
    }

    public void RemoveMainArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove main artist from a published track.");
        }

        _mainArtists.Remove(artistId);
    }

    public void AddFeaturedArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot add featured artist to a published track.");
        }

        _featuredArtists.Add(artistId);
    }

    public void RemoveFeaturedArtist(ArtistId artistId)
    {
        ArgumentNullException.ThrowIfNull(artistId);

        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove featured artist from a published track.");
        }

        _featuredArtists.Remove(artistId);
    }

    public void AddGenre(GenreId genreId)
    {
        ArgumentNullException.ThrowIfNull(genreId);

        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot add genre to a published track.");
        }

        _genres.Add(genreId);
    }

    public void RemoveGenre(GenreId genreId)
    {
        ArgumentNullException.ThrowIfNull(genreId);

        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot remove genre from a published track.");
        }

        _genres.Remove(genreId);
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

    public void ChangeTitle(string newTitle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newTitle);
        TrackTitleRules.Validate(newTitle);
        Title = newTitle;
    }

    public void ChangeReleaseDate(DateTimeOffset newReleaseDate)
        => ReleaseDate = newReleaseDate;

    public void ChangeExplicitContentFlag(bool containsExplicitContent)
        => ContainsExplicitContent = containsExplicitContent;

    public void ChangeTrackNumber(int newTrackNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newTrackNumber);
        TrackNumber = newTrackNumber;
    }

    public void ChangeAudioFile(AudioFileId newAudioFileId, TimeSpan newDuration)
    {
        ArgumentNullException.ThrowIfNull(newAudioFileId);
        if (IsPublished)
        {
            throw new TrackAlreadyPublishedDomainException(
                "Cannot change audio file of a published track.");
        }

        TrackDurationRules.Validate(newDuration);

        AudioFileId = newAudioFileId;
        Duration = newDuration;
    }

    private Track(
        TrackId id,
        string title,
        TimeSpan? duration,
        DateTimeOffset? releaseDate,
        bool containsExplicitContent,
        int trackNumber,
        bool isPublished,
        AudioFileId? audioFileId,
        AlbumId albumId,
        IEnumerable<ArtistId> mainArtists,
        IEnumerable<ArtistId> featuredArtists,
        IEnumerable<GenreId> genres)
        : base(id)
    {
        Title = title;
        Duration = duration;
        ReleaseDate = releaseDate;
        ContainsExplicitContent = containsExplicitContent;
        TrackNumber = trackNumber;
        IsPublished = isPublished;
        AudioFileId = audioFileId;
        AlbumId = albumId;
        _mainArtists = mainArtists.ToHashSet();
        _featuredArtists = featuredArtists.ToHashSet();
        _genres = genres.ToHashSet();
    }

    private Track()
    {
    }
}
