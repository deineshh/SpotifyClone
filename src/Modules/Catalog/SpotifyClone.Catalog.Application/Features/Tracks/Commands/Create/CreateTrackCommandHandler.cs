using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.Create;

internal sealed class CreateTrackCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<CreateTrackCommand, CreateTrackCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<CreateTrackCommandResult>> Handle(
        CreateTrackCommand request,
        CancellationToken cancellationToken)
    {
        var trackId = TrackId.From(Guid.NewGuid());
        var albumId = AlbumId.From(request.AlbumId);

        Album? album = await _unit.Albums.GetByIdAsync(albumId, cancellationToken);
        if (album is null)
        {
            return Result.Failure<CreateTrackCommandResult>(AlbumErrors.NotFound);
        }

        var track = Track.Create(
            trackId,
            request.Title,
            request.ContainsExplicitContent,
            request.TrackNumber,
            album.Id,
            album.Status.IsPublished,
            request.MainArtists.Select(a => ArtistId.From(a)),
            request.FeaturedArtists.Select(a => ArtistId.From(a)),
            request.Genres.Select(g => GenreId.From(g)),
            request.Moods.Select(m => MoodId.From(m)));

        await _unit.Tracks.AddAsync(track, cancellationToken);

        return new CreateTrackCommandResult(trackId.Value);
    }
}
