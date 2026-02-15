using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.Delete;

internal sealed class DeleteAlbumCommandHandler(
    ICatalogUnitOfWork unit,
    ILogger<DeleteAlbumCommandHandler> logger)
    : ICommandHandler<DeleteAlbumCommand, DeleteAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly ILogger<DeleteAlbumCommandHandler> _logger = logger;

    public async Task<Result<DeleteAlbumCommandResult>> Handle(
        DeleteAlbumCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting Album {Id}", request.AlbumId);

        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId),
            cancellationToken);

        if (album is null)
        {
            _logger.LogWarning(
                "Album {Id} not found while deleting", request.AlbumId);

            return Result.Failure<DeleteAlbumCommandResult>(AlbumErrors.NotFound);
        }

        album.PrepareForDeletion();
        await _unit.Albums.DeleteAsync(album, cancellationToken);

        return new DeleteAlbumCommandResult();
    }
}
