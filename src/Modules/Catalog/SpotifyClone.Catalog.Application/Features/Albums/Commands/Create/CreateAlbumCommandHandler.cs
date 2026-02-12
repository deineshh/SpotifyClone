using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.Create;

internal sealed class CreateAlbumCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<CreateAlbumCommand, CreateAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<CreateAlbumCommandResult>> Handle(
        CreateAlbumCommand request,
        CancellationToken cancellationToken)
        => new CreateAlbumCommandResult(Guid.NewGuid());
}
