using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Artists.Commands.UnlinkBanner;

internal sealed class UnlinkBannerFromArtistCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<UnlinkBannerFromArtistCommand, UnlinkBannerFromArtistCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<UnlinkBannerFromArtistCommandResult>> Handle(
        UnlinkBannerFromArtistCommand request,
        CancellationToken cancellationToken)
    {
        Artist? artist = await _unit.Artists.GetByIdAsync(
            ArtistId.From(request.ArtistId),
            cancellationToken);
        if (artist is null)
        {
            return Result.Failure<UnlinkBannerFromArtistCommandResult>(ArtistErrors.NotFound);
        }

        artist.UnlinkBannerIfExists();

        return new UnlinkBannerFromArtistCommandResult();
    }
}
