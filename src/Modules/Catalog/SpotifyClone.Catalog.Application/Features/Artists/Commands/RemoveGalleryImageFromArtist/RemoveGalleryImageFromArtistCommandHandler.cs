using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Artists.Commands.RemoveGalleryImageFromArtist;

internal sealed class RemoveGalleryImageFromArtistCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<RemoveGalleryImageFromArtistCommand, RemoveGalleryImageFromArtistCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<RemoveGalleryImageFromArtistCommandResult>> Handle(
        RemoveGalleryImageFromArtistCommand request,
        CancellationToken cancellationToken)
    {
        Artist? artist = await _unit.Artists.GetByIdAsync(
            ArtistId.From(request.ArtistId),
            cancellationToken);
        if (artist is null)
        {
            return Result.Failure<RemoveGalleryImageFromArtistCommandResult>(AlbumErrors.NotFound);
        }

        artist.RemoveGalleryImage(
            ImageId.From(request.ImageId));

        return new RemoveGalleryImageFromArtistCommandResult();
    }
}
