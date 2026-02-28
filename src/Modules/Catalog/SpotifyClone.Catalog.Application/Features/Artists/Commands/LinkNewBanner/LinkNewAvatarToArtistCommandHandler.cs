using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Artists.Commands.LinkNewBanner;

internal sealed class LinkNewBannerToArtistCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<LinkNewBannerToArtistCommand, LinkNewBannerToArtistCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<LinkNewBannerToArtistCommandResult>> Handle(
        LinkNewBannerToArtistCommand request,
        CancellationToken cancellationToken)
    {
        Artist? artist = await _unit.Artists.GetByIdAsync(
            ArtistId.From(request.ArtistId),
            cancellationToken);
        if (artist is null)
        {
            return Result.Failure<LinkNewBannerToArtistCommandResult>(ArtistErrors.NotFound);
        }

        artist.LinkNewBanner(new ArtistBannerImage(
            ImageId.From(request.ImageId),
            request.ImageWidth,
            request.ImageHeight,
            ImageFileType.From(request.ImageFileType),
            request.ImageSizeInBytes));

        return new LinkNewBannerToArtistCommandResult();
    }
}
