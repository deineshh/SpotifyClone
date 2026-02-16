using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.PublishAlbum;

internal sealed class PublishAlbumCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<PublishAlbumCommandHandler> logger)
    : ICommandHandler<PublishAlbumCommand, PublishAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<PublishAlbumCommandHandler> _logger = logger;

    public async Task<Result<PublishAlbumCommandResult>> Handle(
        PublishAlbumCommand request,
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

            return Result.Failure<PublishAlbumCommandResult>(TrackErrors.NotFound);
        }

        album.Publish(request.ReleaseDate);

        IEnumerable<Track> tracks = await _unit.Tracks.GetByIdsAsync(
            album.Tracks,
            cancellationToken);

        foreach (Track track in tracks)
        {
            track.Publish(request.ReleaseDate);
        }

        return new PublishAlbumCommandResult();
    }
}
