using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Services;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.PublishAlbum;

internal sealed class PublishAlbumCommandHandler(
    ICatalogUnitOfWork unit,
    AlbumTrackDomainService albumTrackDomainService,
    ILogger<PublishAlbumCommandHandler> logger)
    : ICommandHandler<PublishAlbumCommand, PublishAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly AlbumTrackDomainService _albumTrackDomainService = albumTrackDomainService;
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

            return Result.Failure<PublishAlbumCommandResult>(AlbumErrors.NotFound);
        }

        album.Publish(request.ReleaseDate);
        _albumTrackDomainService.ReevaluateAlbumType(album);

        return new PublishAlbumCommandResult();
    }
}
