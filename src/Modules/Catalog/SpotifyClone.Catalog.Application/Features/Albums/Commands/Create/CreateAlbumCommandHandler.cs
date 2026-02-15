using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
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
    {
        var albumId = Guid.NewGuid();

        var album = Album.Create(
            AlbumId.From(albumId),
            request.Title,
            request.MainArtists.Select(a => ArtistId.From(a)));

        await _unit.Albums.AddAsync(album, cancellationToken);

        return new CreateAlbumCommandResult(album.Id.Value);
    }
}
