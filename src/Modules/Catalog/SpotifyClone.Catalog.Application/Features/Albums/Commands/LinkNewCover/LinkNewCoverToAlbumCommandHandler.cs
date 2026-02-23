using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Services;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.LinkNewCover;

internal sealed class LinkNewCoverToAlbumCommandHandler(
    ICatalogUnitOfWork unit,
    AlbumTrackDomainService albumTrackDomainService)
    : ICommandHandler<LinkNewCoverToAlbumCommand, LinkNewCoverToAlbumCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;
    private readonly AlbumTrackDomainService _albumTrackDomainService = albumTrackDomainService;

    public async Task<Result<LinkNewCoverToAlbumCommandResult>> Handle(
        LinkNewCoverToAlbumCommand request,
        CancellationToken cancellationToken)
    {
        Album? album = await _unit.Albums.GetByIdAsync(
            AlbumId.From(request.AlbumId),
            cancellationToken);
        if (album is null)
        {
            return Result.Failure<LinkNewCoverToAlbumCommandResult>(AlbumErrors.NotFound);
        }

        album.LinkNewCover(new AlbumCoverImage(
            ImageId.From(request.ImageId),
            request.ImageWidth,
            request.ImageHeight,
            ImageFileType.From(request.ImageFileType),
            request.ImageSizeInBytes));

        _albumTrackDomainService.TryMarkAlbumAsReadyToPublish(album);

        return new LinkNewCoverToAlbumCommandResult();
    }
}
