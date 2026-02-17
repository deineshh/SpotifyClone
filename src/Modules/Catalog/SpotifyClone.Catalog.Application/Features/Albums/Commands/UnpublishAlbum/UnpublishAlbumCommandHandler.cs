using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.UnpublishAlbum;

internal sealed class UnpublishAlbumCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<UnpublishAlbumCommandHandler> logger)
    : ICommandHandler<UnpublishAlbumCommand, UnpublishAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<UnpublishAlbumCommandHandler> _logger = logger;

    public async Task<Result<UnpublishAlbumCommandResult>> Handle(
        UnpublishAlbumCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing Album {Id}", request.AlbumId);

        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId),
            cancellationToken);

        if (album is null)
        {
            _logger.LogWarning(
                "Album {Id} not found while publishing", request.AlbumId);

            return Result.Failure<UnpublishAlbumCommandResult>(TrackErrors.NotFound);
        }

        album.Unpublish();

        IEnumerable<Track> tracks = await _unit.Tracks.GetByIdsAsync(
            album.Tracks,
            cancellationToken);

        foreach (Track track in tracks)
        {
            // No need for an exception if a track is not published, just skip it then,
            // because maybe the track is not published by purpose.
            if (track.Status.IsPublished)
            {
                track.Unpublish();
            }
        }

        return new UnpublishAlbumCommandResult();
    }
}
