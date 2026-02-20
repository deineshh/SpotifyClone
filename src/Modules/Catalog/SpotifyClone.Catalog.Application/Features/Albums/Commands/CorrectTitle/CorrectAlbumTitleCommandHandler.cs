using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.CorrectTitle;

internal sealed class CorrectAlbumTitleCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<CorrectAlbumTitleCommand, CorrectAlbumTitleCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<CorrectAlbumTitleCommandResult>> Handle(
        CorrectAlbumTitleCommand request,
        CancellationToken cancellationToken)
    {
        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId),
            cancellationToken);

        if (album is null)
        {
            return Result.Failure<CorrectAlbumTitleCommandResult>(AlbumErrors.NotFound);
        }

        album.CorrectTitle(request.Title);

        return new CorrectAlbumTitleCommandResult();
    }
}
