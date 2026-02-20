using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.RescheduleRelease;

internal sealed class RescheduleAlbumReleaseCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<RescheduleAlbumReleaseCommand, RescheduleAlbumReleaseCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<RescheduleAlbumReleaseCommandResult>> Handle(
        RescheduleAlbumReleaseCommand request,
        CancellationToken cancellationToken)
    {
        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId),
            cancellationToken);

        if (album is null)
        {
            return Result.Failure<RescheduleAlbumReleaseCommandResult>(AlbumErrors.NotFound);
        }

        album.RescheduleRelease(request.ReleaseDate);

        return new RescheduleAlbumReleaseCommandResult();
    }
}
